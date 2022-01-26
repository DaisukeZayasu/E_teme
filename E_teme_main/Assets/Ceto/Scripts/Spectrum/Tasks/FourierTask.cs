﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Ceto.Common.Threading.Tasks;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{

	public class FourierTask : ThreadedTask
	{

		FourierCPU m_fourier;

        DisplacementBufferCPU m_buffer;

        int m_numGrids;

		int m_index;

        /// <summary>
        /// The data generated from the init task
        /// and need to have the fouier transform applied 
        /// to it.
        /// </summary>
		IList<Vector4[]> m_data;

        /// <summary>
        /// The data generated by the fouier transform and then
        /// repacked to a format that can be copied to the texture.
        /// </summary>
		Color[] m_results;

        /// <summary>
        /// The texture that will hold the displacements and 
        /// will have the results copied to it.
        /// </summary>
        Texture2D m_map;

        /// <summary>
        /// If true the vector4 contains two complex numbers in the xy and zw,
        /// else it contains one in the xy.
        /// </summary>
		bool m_doublePacked;

		public FourierTask(WaveSpectrumBufferCPU buffer, FourierCPU fourier, int index, int numGrids) 
            : base(true)
		{

			if(m_index == -1)
				throw new InvalidOperationException("Index can be -1. Fourier for multiple buffers is not being used");

            if(!(buffer is DisplacementBufferCPU)) //TODO - fix me
                throw new InvalidOperationException("Fourier task currently only designed for displacement buffers");

            m_buffer = buffer as DisplacementBufferCPU;

			m_fourier = fourier;

			m_index = index;

            m_numGrids = numGrids;

            WaveSpectrumBufferCPU.Buffer b = m_buffer.GetBuffer(m_index);
			
			m_data = b.data;
			m_results = b.results;
            m_map = b.map;
			m_doublePacked = b.doublePacked;

		}

        /// <summary>
        /// Reset task to inital state so it can be reran by the scheduler.
        /// Saves having to create a new object each frame.
        /// </summary>
        public void Reset(int index, int numGrids)
        {

            base.Reset();

            if (m_index == -1)
                throw new InvalidOperationException("Index can be -1. Fourier for multiple buffers is not being used");

            m_index = index;

            m_numGrids = numGrids;

            WaveSpectrumBufferCPU.Buffer b = m_buffer.GetBuffer(m_index);

            m_data = b.data;
            m_results = b.results;
            m_map = b.map;
            m_doublePacked = b.doublePacked;
        }

		public override void Start()
		{
			base.Start();
		}
		
		public override IEnumerator Run()
		{

			PerformSingleFourier();

			FinishedRunning();
			return null;
		}
		
		public override void End()
		{

			base.End();

            m_map.SetPixels(m_results);
            m_map.Apply();
		}

        void PerformSingleFourier()
		{

			//Always start writing at buffer index 0 and the end read buffer should always end up at index 1.
			const int write = 0;
			int read = -1;

			if(m_doublePacked)
				read = m_fourier.PeformFFT_DoublePacked(write, m_data, this);
			else
				read = m_fourier.PeformFFT_SinglePacked(write, m_data, this);

            if (Cancelled) return;
			
			if(read != WaveSpectrumBufferCPU.READ)
				throw new InvalidOperationException("Fourier transform did not result in the read buffer at index " + WaveSpectrumBufferCPU.READ);

		    ProcessData(m_index, m_results, m_data[read], m_numGrids);
				
		}

        /// <summary>
        /// After the fourier tasks creates the data this function is 
        /// called which allows the buffer to further process the results if needed.
        /// Used to sort the results into the displacement buffers that are 
        /// sampled from for the wave queries and copy into the result array
        /// which gets copied into the textures. This is needed as the packing of 
        /// the data thats optimal for the FFT is not the same as what optimal for 
        /// sampling from the textures and displacement buffer.
        /// </summary>
        void ProcessData(int index, Color[] result, Vector4[] data, int numGrids)
        {

            int CHANNELS = QueryDisplacements.CHANNELS;
            int size = m_buffer.Size;

            InterpolatedArray2f[] displacements = m_buffer.GetWriteDisplacements();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int j = x + y * size;
                    int IDX = j * CHANNELS;

                    if (numGrids == 1)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = 0.0f;
                        result[j].a = 0.0f;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                        }
                    }
                    else if (numGrids == 2)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = data[j].z;
                        result[j].a = data[j].w;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                            displacements[1].Data[IDX + 1] = result[j].g;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                            displacements[1].Data[IDX + 0] += result[j].b;
                            displacements[1].Data[IDX + 2] += result[j].a;
                        }
                    }
                    else if (numGrids == 3)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = data[j].z;
                        result[j].a = data[j].w;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                            displacements[1].Data[IDX + 1] = result[j].g;
                            displacements[2].Data[IDX + 1] = result[j].b;
                            displacements[3].Data[IDX + 1] = result[j].a;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                            displacements[1].Data[IDX + 0] += result[j].b;
                            displacements[1].Data[IDX + 2] += result[j].a;
                        }
                        else if (index == 2)
                        {
                            displacements[2].Data[IDX + 0] += result[j].r;
                            displacements[2].Data[IDX + 2] += result[j].g;
                        }
                    }
                    else if (numGrids == 4)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = data[j].z;
                        result[j].a = data[j].w;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                            displacements[1].Data[IDX + 1] = result[j].g;
                            displacements[2].Data[IDX + 1] = result[j].b;
                            displacements[3].Data[IDX + 1] = result[j].a;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                            displacements[1].Data[IDX + 0] += result[j].b;
                            displacements[1].Data[IDX + 2] += result[j].a;
                        }
                        else if (index == 2)
                        {
                            displacements[2].Data[IDX + 0] += result[j].r;
                            displacements[2].Data[IDX + 2] += result[j].g;
                            displacements[3].Data[IDX + 0] += result[j].b;
                            displacements[3].Data[IDX + 2] += result[j].a;
                        }
                    }
                    else
                    {
                        result[j].r = 0.0f;
                        result[j].g = 0.0f;
                        result[j].b = 0.0f;
                        result[j].a = 0.0f;
                    }

                }

            }

        }

    }

}













