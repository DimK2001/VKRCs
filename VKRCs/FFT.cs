using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace VKRCs
{
	public class FFT
	{
        /*public static double[] ZeroPad(double[] _input)
        {
            if (isPowerOfTwo(_input.Length))
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
            //Array.Copy(_input, 0, _padded, _difference / 2, _input.Length);
            Array.Copy(_input, _padded, _input.Length);
            Array.Copy(_input, 0, _padded, _input.Length, _difference);

            return _padded;
        }*/
        public static Complex[] ZeroPad(Complex[] _input)
        {
            if (isPowerOfTwo(_input.Length))
            {
                return _input;
            }

            int _targetLength = 2;
            while (_targetLength < _input.Length)
            {
                _targetLength *= 2;
            }

            double[] x = new double[_input.Length];
            for (int i = 0; i < _input.Length; ++i)
            {
                x[i] = i;
            }
            double[] _valsX = new double[_targetLength];
            double _diff = Convert.ToDouble(_input.Length) / Convert.ToDouble(_targetLength);
            for (int i = 0; i < _targetLength; ++i)
            {
                _valsX[i] = i * _diff;
            }
            Complex[] result = LagrangeInterpolation(x, _input, _valsX);
            return result;

            /*int _difference = _targetLength - _input.Length;
            Complex[] _padded = new Complex[_targetLength];
            //Array.Copy(_input, 0, _padded, _difference / 2, _input.Length);
            Array.Copy(_input, _padded, _input.Length);
            Array.Copy(_input, 0, _padded, _input.Length, _difference);

            return _padded;*/

        }
        public static Complex[] fft(Complex[] _x)
		{
			int n = _x.Length;
			//Базовый случай
			if (n == 1) return new Complex[] { _x[0] };

			//Проверка n - степень 2, для алгоритма Кули — Тьюки
			if (!isPowerOfTwo(n))
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

        public static Complex[] dft(Complex[] _x)
        {
            List<Complex> _koeffs = new List<Complex>();
            double n = _x.Length;

            //Цикл вычисления коэффициентов 
            for (int u = 0; u < _x.Length; u++)
            {
                //Цикл суммы 
                Complex _summa = new Complex();
                for (int k = 0; k < n; k++)
                {
                    Complex S = new Complex(_x[k].Real, k);
                    double _koeff = -2 * Math.PI * u * k / n;
                    Complex e = new Complex(Math.Cos(_koeff), Math.Sin(_koeff));
                    _summa += (S * e);
                }
                _koeffs.Add(_summa / n);
            }
            return _koeffs.ToArray();
        }

        private static bool isPowerOfTwo(long x)
        {
            return (x & (x - 1)) == 0;
        }

        private static Complex lagrange(double[] x, Complex[] y, Complex _xval)
        {
            Complex _yval = 0.0;
            Complex _yInter = y[0];
            for (int i = 0; i < x.Length; i++)
            {
                _yInter = y[i];
                for (int j = 0; j < x.Length; j++)
                {
                    if (i != j)
                    {
                        _yInter *= (_xval - x[j]) / (x[i] - x[j]);
                    }
                }
                _yval += _yInter;
            }
            return _yval;
        }

        public static Complex[] LagrangeInterpolation(double[] x, Complex[] y, double[] _xvals)
        {
            Complex[] _yvals = new Complex[_xvals.Length];
            for (int i = 0; i < _xvals.Length; i++)
            {
                _yvals[i] = lagrange(x, y, _xvals[i]);
            }
            return _yvals;
        }
    }
}
