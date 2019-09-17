using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEEC.MI.High_Precision
{
    public class Class1
    {


    }

    class testDataReset
    {
        private float _FRE;
        private float _An;
        private float _Ax;
        private float _Ph;
        private float _Umean;
        // private double _Df;

        private double _Df_tan;
        private double _Df_tan_75;
        private double _Df_tan_Per100;
        private double _Df_tan_75_Per100;
        private double _Pf_cos;
        private double _Pf_cos_75;
        private double _Pf_cos_Per100;
        private double _Pf_cos_75_Per100;
        private double _Urms;
        private double _Urms_sqrt3;
        private double _Urect_mean;
        private double _In_rms;
        private double _Ix_rms;
        private double _Fre;
        private double _Fre_database;
        private double _Zx;
        private double _Yx;
        private double _I_mag_LP;
        private double _sita_Zx;
        private double _Cx;
        private double _S;
        private double _P;
        private double _Q;
        private double _P_25;
        private double _P_10;
        private double _Cn;
        private double _temper;

        private double _I_fe_rp;
        private double _Cp;
        private double _Rp_CR;
        private double _Rs_CR;
        private double _Cs;
        private double _Ls;
        private double _Rs_LR;
        private double _Lp;
        private double _Rp_LR;

        public testDataReset(float FRE, float An, float Ax, float Ph, float Umean, double Cn, double temper)
        {
            An /= 1000;
            Ax /= 1000;

            this._FRE = FRE;
            // this._age = Age;
            this._An = An;
            this._Ax = Ax;
            this._Ph = Ph;
            this._Umean = Umean;
            double w = 2 * Math.PI * FRE;

            this._Df_tan = Math.Tan(Ph);
            this._Df_tan_75 = _Df_tan * Math.Pow(1.3, (Convert.ToDouble((75 - temper)) / 10));//t1为设定的值
            //                                                                                                              // string result = string.Format("{0:0.00%}", percent);//得到5.88%
            this._Df_tan_Per100 = _Df_tan * 100;
            //this._Df_tan_75_Per100 = Convert.ToDouble(string.Format("{0:0.00%}", _Df_tan_75));
            this._Df_tan_75_Per100 = _Df_tan_75 * 100;

            this._Pf_cos = Math.Cos(Math.PI / 2 - Ph);
            this._Pf_cos_75 = Math.Sqrt((_Df_tan_75 * _Df_tan_75) / (_Df_tan_75 * _Df_tan_75 + 1));
            //this._Pf_cos_Per100 = Convert.ToDouble(string.Format("{0:0.00%}", _Pf_cos));
            //this._Pf_cos_75_Per100 = Convert.ToDouble(string.Format("{0:0.00%}", _Pf_cos_75));
            this._Pf_cos_Per100 = _Pf_cos * 100;
            this._Pf_cos_75_Per100 = _Pf_cos_75 * 100;
            this._Urms = An / (2 * Math.PI * FRE * Cn);/// Math.Sqrt(2)
            this._Urms_sqrt3 = _Urms / Math.Sqrt(3);
            this._Urect_mean = _Umean;
            this._In_rms = An;
            this._Ix_rms = Ax;

            this._Fre = FRE;
            this._Fre_database = FRE;

            this._Zx = An * (2 * Math.PI * FRE * Cn * Ax);//提供接口
            this._Yx = (2 * Math.PI * FRE * Cn * Ax) / An;//提供接口
            this._sita_Zx = Ph;

            this._Cx = Ax * Cn / (An * Math.Cos(Ph));
            this._Cn = Cn;//提供的值，修改

            this._S = Ax * An / (2 * Math.PI * FRE * Cn);//Cn
            this._P = Ax * An / (2 * Math.PI * FRE * Cn) * Math.Cos(Ph);
            this._Q = Ax * An / (2 * Math.PI * FRE * Cn) * Math.Sin(Ph);
            this._temper = temper;//提供的值，修改
            this._P_10 = Math.PI;//提供的值，修改
            this._P_25 = Math.PI;//提供的值，修改

            this._sita_Zx = Math.PI / 2 - Ph;
            this._I_mag_LP = Ax * Math.Cos(Ph);
            this._I_fe_rp = Ax * Math.Sin(Ph);
            this._Cp = _Cx;
            this._Rp_CR = An * Math.Sin(Ph) / (w * Cn * Ax);
            this._Cs = Cn * (1 + Math.Tan(Ph) * Math.Tan(Ph));
            this._Rs_CR = _Cp * (Math.Tan(Ph) * Math.Tan(Ph) / (1 + Math.Tan(Ph) * Math.Tan(Ph)));
            this._Ls = Math.Sqrt(1 / (_Cs * w * w));
            this._Rs_LR = _Rs_CR;
            this._Lp = Math.Sqrt(1 / (_Cp * w * w));
            this._Rp_LR = _Rp_CR;


        }

        public double Df_tan
        {
            get { return _Df_tan; }

        }
        public float FRE
        {
            get { return _FRE; }
        }
        public float An
        {
            get { return _An; }
        }
        public float Ax
        {
            get { return _Ax; }
        }
        public float Ph
        {
            get { return _Ph; }
        }
        public float Umean
        {
            get { return _Umean; }
        }

        public double I_fe_rp
        {
            get { return _I_fe_rp; }
        }
        public double Cp
        {
            get { return _Cp; }
        }
        public double Rp_CR
        {
            get { return _Rp_CR; }
        }
        public double Rs_CR
        {
            get { return _Rs_CR; }
        }
        public double Cs
        {
            get { return _Cs; }
        }
        public double Ls
        {
            get { return _Ls; }
        }
        public double Rs_LR
        {
            get { return _Rs_LR; }
        }
        public double Lp
        {
            get { return _Lp; }
        }
        public double Rp_LR
        {
            get { return _Rp_LR; }
        }
        //..........................




        public double Zx
        {
            get { return _Zx; }
        }
        public double Yx
        {
            get { return _Yx; }
        }


        public double Df_tan_75
        {
            get { return _Df_tan_75; }
        }
        public double Df_tan_Per100
        {
            get { return _Df_tan_Per100; }
        }
        public double Df_tan_75_Per100
        {
            get { return _Df_tan_75_Per100; }
        }
        public double Pf_cos
        {
            get { return _Pf_cos; }
        }
        public double Pf_cos_75
        {
            get { return _Pf_cos_75; }
        }
        public double Pf_cos_Per100
        {
            get { return _Pf_cos_Per100; }
        }
        public double Pf_cos_75_Per100
        {
            get { return _Pf_cos_75_Per100; }
        }
        public double Urms
        {
            get { return _Urms; }
        }
        public double Urms_sqrt3
        {
            get { return _Urms_sqrt3; }
        }
        public double Urect_mean
        {
            get { return _Urect_mean; }
        }
        public double In_rms
        {
            get { return _In_rms; }
        }
        public double Ix_rms
        {
            get { return _Ix_rms; }
        }
        public double Fre
        {
            get { return _Fre; }
        }
        public double Fre_database
        {
            get { return _Fre_database; }
        }
        public double sita_Zx
        {
            get { return _sita_Zx; }
        }
        public double Cx
        {
            get { return _Cx; }
        }
        public double S
        {
            get { return _S; }
        }
        public double P
        {
            get { return _P; }
        }
        public double Q
        {
            get { return _Q; }
        }
        public double P_25
        {
            get { return _P_25; }
        }
        public double P_10
        {
            get { return _P_10; }
        }
        public double Cn
        {
            get { return _Cn; }
        }
        // _I_mag_LP

        public double temper
        { get { return _temper; } }

        public double I_mag_LP
        {
            get { return _I_mag_LP; }
        }


    }
}
