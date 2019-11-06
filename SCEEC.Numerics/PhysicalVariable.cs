using SCEEC.Numerics.Quantities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SCEEC.Numerics
{
    /// <summary>
    /// 物理量
    /// </summary>
    public class PhysicalVariable
    {
        public PhysicalVariable() { }

        public static PhysicalVariable FromString(string value)
        {
            return NumericsConverter.Text2Value(value);
        }
        /// <summary>
        /// 值
        /// </summary>
        public double? value;
        /// <summary>
        /// 类型
        /// </summary>
        public QuantityName PhysicalVariableType;
        /// <summary>
        /// 源文本
        /// </summary>
        public string OriginText;
        /// <summary>
        /// 有效长度
        /// </summary>
        public int? EffectiveLength;
        /// <summary>
        /// 转换为SI词头的有效位数表示方式文本
        /// </summary>
        /// <returns>物理量的带有SI词头的有效位数表示方式文本</returns>
        public override string ToString()
        {
            return NumericsConverter.Value2Text(this);
        }
        /// <summary>
        /// 转换为SI词头的有效位数表示方式文本
        /// </summary>
        /// <param name="percentage">百分号显示</param>
        /// <param name="positiveSign">正值显示+号</param>
        /// <returns>物理量的带有SI词头的有效位数表示方式文本</returns>
        public string ToString(bool percentage = false, bool positiveSign = false, bool usePrefix = true)
        {
            return NumericsConverter.Value2Text(this, percentage, positiveSign, usePrefix);
        }

        

        public static implicit operator PhysicalVariable(string value)
        {
            return NumericsConverter.Text2Value(value);
        }

        public static implicit operator double(PhysicalVariable value)
        {
            return (value.value != null) ? (double)value.value : 0.0;
        }
        public static implicit operator string(PhysicalVariable value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public Unit Unit
        {
            get
            {
                return ((Unit)this.PhysicalVariableType);
            }
            set
            {
                this.PhysicalVariableType = (QuantityName)value;
            }
        }
        /// <summary>
        /// 单位符号
        /// </summary>
        public Symbol Symbol
        {
            get
            {
                return ((Symbol)this.PhysicalVariableType);
            }
            set
            {
                this.PhysicalVariableType = (QuantityName)value;
            }
        }
    }
}
