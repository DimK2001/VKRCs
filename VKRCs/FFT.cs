using System;
using System.Collections.Generic;
using System.Numerics;

namespace vkrC
{
	public class FFT
	{
		public static Complex[] fft(Complex[] _x)
		{
			int n = _x.Length;
			//Базовый случай
			if (n == 1) return new Complex[] { _x[0] };

			//Проверка n - степень 2, для алгоритма Кули — Тьюки
			if (n % 2 != 0)
			{
				throw new ArgumentException("n не делится на 2");
			}

			//Для четных
			Complex[] even = new Complex[n / 2];
			//Для нечетных
			Complex[] odd = new Complex[n / 2];
			for (int k = 0; k < n / 2; ++k)
			{
				even[k] = _x[2 * k];
				odd[k] = _x[2 * k + 1];
			}
			Complex[] evenFFT = fft(even);
			Complex[] oddFFT = fft(odd);

			//Объединение
			Complex[] freqs = new Complex[n];
			for (int k = 0; k < n / 2; ++k)
			{
				double kth = -2 * k * Math.PI / n;
				Complex complexExp = new Complex(Math.Cos(kth), Math.Sin(kth)) * oddFFT[k];
				freqs[k] = evenFFT[k] + complexExp;
				freqs[k + n / 2] = evenFFT[k] - complexExp;
			}
			return freqs;
		}
	}
}
