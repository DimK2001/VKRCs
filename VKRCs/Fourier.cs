using System;
using System.Collections.Generic;
using System.Numerics;

namespace VKRCs
{
	public class Fourier
	{
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

        }
        public static Complex[] fft(Complex[] _x)
		{
			int N = _x.Length;
			//Базовый случай
			if (N == 1) return new Complex[] { _x[0] };

			//Для четных
			Complex[] _even = new Complex[N / 2];
			//Для нечетных
			Complex[] _odd = new Complex[N / 2];
			for (int k = 0; k < N / 2; ++k)
			{
				_even[k] = _x[2 * k];
				_odd[k] = _x[2 * k + 1];
			}
			Complex[] _evenFFT = fft(_even);
			Complex[] _oddFFT = fft(_odd);

			//Объединение
			Complex[] _freqs = new Complex[N];
            double _arg = -2.0 * Math.PI / (double)N;
            for (int k = 0; k < N / 2; ++k)
			{
				double _kth = _arg * (double)k;
				Complex _complexExp = new Complex(Math.Cos(_kth), Math.Sin(_kth)) * _oddFFT[k];
				_freqs[k] = _evenFFT[k] + _complexExp;
				_freqs[k + N / 2] = _evenFFT[k] - _complexExp;
            }
			return _freqs;
		}

        public static Complex[] dft(Complex[] _x)
        {
            List<Complex> _koeffs = new List<Complex>();
            double N = _x.Length;

            double _arg = -2.0 * Math.PI / (double)N;
            //Цикл вычисления коэффициентов 
            for (int n = 0; n < N; n++)
            {
                //Цикл суммы 
                double _argCicle = _arg * (double)n;
                Complex _summ = new Complex();
                for (int k = 0; k < N; k++)
                {
                    double _koeff = _argCicle * k;
                    Complex e = new Complex(Math.Cos(_koeff), Math.Sin(_koeff));
                    _summ += (_x[k] * e);
                }
                _koeffs.Add(_summ / N);
            }
            return _koeffs.ToArray();
        }
        public static Complex[] dftPolar(Complex[] _x)
        {
            int N = _x.Length;
            Complex[] _output = new Complex[N];

            double _arg = -2.0 * Math.PI / (double)N;
            for (int n = 0; n < N; n++)
            {
                double _argCicle = _arg * (double)n;
                _output[n] = new Complex();
                for (int k = 0; k < N; k++)
                {
                    _output[n] += _x[k] * Complex.FromPolarCoordinates(1, _argCicle * (double)k);
                }
                _output[n] /=  N;
            }
            return _output;
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
