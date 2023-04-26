using System;
using System.Numerics;

namespace VKRCs
{
	public class FFT
	{
        public static double[] ZeroPad(double[] _input)
        {
            if (_input.Length % 2 == 0)
            {
                return _input;
            }

            int _targetLength = 1;
            while (_targetLength < _input.Length)
            {
                _targetLength *= 2;
            }

            int _difference = _targetLength - _input.Length;
            double[] _padded = new double[_targetLength];
            Array.Copy(_input, 0, _padded, _difference / 2, _input.Length);

            return _padded;
        }
        public static Complex[] ZeroPad(Complex[] _input)
        {
            if (_input.Length % 2 == 0)
            {
                return _input;
            }

            int _targetLength = 1;
            while (_targetLength < _input.Length)
            {
                _targetLength *= 2;
            }

            int _difference = _targetLength - _input.Length;
            Complex[] _padded = new Complex[_targetLength];
            Array.Copy(_input, 0, _padded, _difference / 2, _input.Length);

            return _padded;
        }
        public static Complex[] fft(Complex[] _x)
		{
			int n = _x.Length;
			//Базовый случай
			if (n == 1) return new Complex[] { _x[0] };

			//Проверка n - степень 2, для алгоритма Кули — Тьюки
			if (n % 2 != 0)
			{
				throw new ArgumentException("n не является степенью 2х");
			}

			//Для четных
			Complex[] _even = new Complex[n / 2];
			//Для нечетных
			Complex[] _odd = new Complex[n / 2];
			for (int k = 0; k < n / 2; ++k)
			{
				_even[k] = _x[2 * k];
				_odd[k] = _x[2 * k + 1];
			}
			Complex[] _evenFFT = fft(_even);
			Complex[] _oddFFT = fft(_odd);

			//Объединение
			Complex[] _freqs = new Complex[n];
			for (int k = 0; k < n / 2; ++k)
			{
				double _kth = -2 * k * Math.PI / n;
				Complex _complexExp = new Complex(Math.Cos(_kth), Math.Sin(_kth)) * _oddFFT[k];
				_freqs[k] = _evenFFT[k] + _complexExp;
				_freqs[k + n / 2] = _evenFFT[k] - _complexExp;
			}
			return _freqs;
		}
	}
}
