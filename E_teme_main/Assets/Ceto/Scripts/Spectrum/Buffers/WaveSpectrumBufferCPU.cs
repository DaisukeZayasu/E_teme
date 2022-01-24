﻿using UnityEngine;
using System;
using System.Collections.Generic;

using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Threading.Tasks;

#pragma warning disable 162, 429

namespace Ceto
{

    /// <summary>
    /// A buffer that uses FFT on the CPU to transform
    /// the spectrum. The type of data produced depends on the
    /// initialization provided by the superclass.
    /// </summary>
	public abstract class WaveSpectrumBufferCPU : WaveSpectrumBuffer
	{
        //Dont change these
		public const int READ = 1;
		public const int WRITE = 0;
		

        /// <summary>
        /// Holds the actual buffer data.
        /// </summary>
		public class Buffer
		{
            //Array to hold the read/write data
			public IList<Vector4[]> data;
            //The results from the FFT end up in here.
			public Color[] results;
            //When the FFT task finishes the results get written to the texture
			public Texture2D map;
			//Is this buffer disabled.
			public bool disabled;
			//Is the data double packed (two complex numbers in one vector4).
			public bool doublePacked;
		}

        /// <summary>
        /// Has the data requested been created.
        /// CPU buffers can create their data 
        /// on multiple threads. Done is true
        /// when all the threads finish. 
        /// </summary>
		public override bool Done { get { return IsDone(); } }

        /// <summary>
        /// The fourier size of the buffer.
        /// </summary>
		public override int Size { get { return m_fourier.size; } }

        /// <summary>
        /// Does this buffer run on the GPU. Never true.
        /// </summary>
		public override bool IsGPU { get { return false; } }

        /// <summary>
        /// WTable.
        /// </summary>
		public Color[] WTable { get; private set; }

        /// <summary>
        /// The buffers generated by this object.
        /// </summary>
		protected Buffer[] m_buffers;

        /// <summary>
        /// Does the actual FFT on the CPU.
        /// </summary>
		FourierCPU m_fourier;

        /// <summary>
        /// Runs the fourier tasks.
        /// </summary>
		Scheduler m_scheduler;

        /// <summary>
        /// The fourier task that are currently running.
        /// </summary>
		List<FourierTask> m_fourierTasks;

        /// <summary>
        /// The task to initialize the data.
        /// Is created by the superclass.
        /// </summary>
		protected InitSpectrumDisplacementsTask m_initTask;
		
		public WaveSpectrumBufferCPU(int size, int numBuffers, Scheduler scheduler)
		{

			m_buffers = new Buffer[numBuffers];

			m_fourier = new FourierCPU(size);

            m_fourierTasks = new List<FourierTask>(3);
            m_fourierTasks.Add(null);
            m_fourierTasks.Add(null);
            m_fourierTasks.Add(null);

            m_scheduler = scheduler;
			
			for(int i = 0; i < numBuffers; i++)
			{
				m_buffers[i] = CreateBuffer(size);
			}

		}
        /// <summary>
        /// Create a buffer for this fourier size.
        /// A buffer requires two arrays.
        /// During the FFT one arrays is written into  
        /// while the other is read from and then they swap.
        /// This is the read/write method (also know as ping/pong).
        /// </summary>
		Buffer CreateBuffer(int size)
		{

			Buffer buffer = new Buffer();

			buffer.doublePacked = true;
			buffer.data = new List<Vector4[]>();
			buffer.data.Add(new Vector4[size * size]);
			buffer.data.Add(new Vector4[size * size]);

			buffer.results = new Color[size * size];

			buffer.map = new Texture2D(size, size, TextureFormat.RGBAFloat, false, true);
			buffer.map.wrapMode = TextureWrapMode.Repeat;
			buffer.map.filterMode = FilterMode.Bilinear;
			buffer.map.hideFlags = HideFlags.HideAndDontSave;
            buffer.map.name = "Ceto Wave Spectrum CPU Buffer";

			buffer.map.SetPixels(buffer.results);
			buffer.map.Apply();

			return buffer;

		}

        /// <summary>
        /// Release the buffers.
        /// </summary>
        public override void Release()
        {
            int count = m_buffers.Length;
            for(int i = 0; i < count; i++)
            {
                UnityEngine.Object.Destroy(m_buffers[i].map);
                m_buffers[i].map = null;
            }

        }

        /// <summary>
        /// Get the read texture at this idx.
        /// If buffer is disabled or not a valid index
        /// a blank texture is returned.
        /// </summary>
		public override Texture GetTexture(int idx)
		{

			if(idx < 0 || idx >= m_buffers.Length) return Texture2D.blackTexture;

			if(m_buffers[idx].disabled) return Texture2D.blackTexture;

			return m_buffers[idx].map;
		}

        /// <summary>
        /// Get the write buffer for this idx.
        /// </summary>
		public Vector4[] GetWriteBuffer(int idx)
		{
			if(idx < 0 || idx >= m_buffers.Length) return null;

			if(m_buffers[idx].disabled) return null;

			return m_buffers[idx].data[WRITE];
		}

        /// <summary>
        /// Get the read buffer for this idx.
        /// </summary>
		public Vector4[] GetReadBuffer(int idx)
		{
			if(idx < 0 || idx >= m_buffers.Length) return null;

			if(m_buffers[idx].disabled) return null;

			return m_buffers[idx].data[READ];
		}

		/// <summary>
		/// Get the buffer for this idx.
		/// </summary>
		public Buffer GetBuffer(int idx)
		{
			if(idx < 0 || idx >= m_buffers.Length) return null;
			
			if(m_buffers[idx].disabled) return null;
			
			return m_buffers[idx];
		}

        /// <summary>
        /// Enables the data for the buffer at this idx.
        /// If idx is -1 all the buffers will be enabled.
        /// </summary>
		public override void EnableBuffer(int idx)
		{

            int count = m_buffers.Length;
            if (idx < -1 || idx >= count) return;
			
			if(idx == -1)
			{
				for(int i = 0; i < count; i++)
				{
					m_buffers[i].disabled = false;
				}
			}
			else
			{
				m_buffers[idx].disabled = false;
				
			}
			
		}

        /// <summary>
        /// Disables the data for the buffer at this idx.
        /// If idx is -1 all the buffers will be enabled.
        /// </summary>
		public override void DisableBuffer(int idx)
		{

            int count = m_buffers.Length;
            if (idx < -1 || idx >= count) return;
			
			if(idx == -1)
			{
				for(int i = 0; i < count; i++)
				{
					m_buffers[i].disabled = true;
				}
			}
			else
			{
				m_buffers[idx].disabled = true;
			}
		}

        /// <summary>
        /// Returns true if all the
        /// fourier task ran have finished.
        /// </summary>
		public bool IsDone()
		{

            //if init task is null the buffer has
            //never been ran so it counts as being done.
			if(m_initTask == null) return true;

            //Else the buffer count as being done if all its tasks are done.

			if(!m_initTask.Done) return false;

            int count = m_fourierTasks.Count;
            for(int i = 0; i < count; i++)
			{
                if(m_fourierTasks[i] == null) continue;
				if(!m_fourierTasks[i].Done) return false;
			}

			return true;

		}

        /// <summary>
        /// Returns the number of enabled buffers.
        /// </summary>
		public override int EnabledBuffers()
		{

			int enabled = 0;
			int len = m_buffers.Length;
			for(int i = 0; i < len; i++)
			{
				if(!m_buffers[i].disabled)
					enabled++;
			}
			
			return enabled;
		}

        /// <summary>
        /// Is this buffer enabled.
        /// </summary>
        public override bool IsEnabledBuffer(int idx)
        {
            if (idx < 0 || idx >= m_buffers.Length) return false;

            return !m_buffers[idx].disabled;

        }

        /// <summary>
        /// Creates the data for this conditions spectrum for this time value.
        /// </summary>
		public override void Run(WaveSpectrumCondition condition, float time)
		{

			//Can only run if all tasks have finished.
			if(!IsDone()) 
				throw new InvalidOperationException("Can not run when there are tasks that have not finished");

			TimeValue = time;
			HasRun = true;
			BeenSampled = false;

            //If no buffers are enabled nothing to do.
			if(EnabledBuffers() == 0) return;

            //Create the Initialization task.
            //This is created by the superclass
            //to the m_initTask object.
			Initilize(condition, time);

            //Must run init task first if mutithreading disabled
			if (Ocean.DISABLE_FOURIER_MULTITHREADING)
			{
                //Multithreading disabled, run now on this thread.
                m_initTask.Start();
                m_initTask.Run();
                m_initTask.End();
			}

			int numBuffers = m_buffers.Length;

            //float t = Time.realtimeSinceStartup;

            //for each buffer run a fourier task.
			for(int i = 0; i < numBuffers; i++)
			{
				if(m_buffers[i].disabled) continue;

                if (m_fourierTasks[i] == null)
                    m_fourierTasks[i] = new FourierTask(this, m_fourier, i, m_initTask.NumGrids);
                else
                    m_fourierTasks[i].Reset(i, m_initTask.NumGrids);

                FourierTask task = m_fourierTasks[i];

                if (task.Done)
                    throw new InvalidOperationException("Fourier task should not be done before running");

                if (Ocean.DISABLE_FOURIER_MULTITHREADING)
				{
                    //Multithreading disabled, run now on this thread.
					task.Start();
					task.Run();
					task.End();
				}
				else
				{
                    //Multithreading enabled, run now on another thread.

                    //If init task has not finished yet the
                    //fourier task must wait on it to finish.
					task.RunOnStopWaiting = true;
					task.WaitOn(m_initTask);
					m_scheduler.AddWaiting(task);
		
				}

			}

            //Debug.Log("Run Fourier time = " + (Time.realtimeSinceStartup - t) * 1000.0f + " tasks = " + m_fourierTasks.Count);

            //Must run init task last if mutithreading not disabled
			if (!Ocean.DISABLE_FOURIER_MULTITHREADING)
			{
				//Must run init task after tasks the are waiting
				//have been added. Otherwise init task may finish
				//before there other tasks have been added and a 
				//exception will be thrown.
				m_initTask.NoFinish = true;
				m_scheduler.Run(m_initTask);
			}

			
		}

	}

}




