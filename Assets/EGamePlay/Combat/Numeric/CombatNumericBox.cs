using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 整形数值
    /// </summary>
    public class IntNumeric
    {
        public int Value { get; private set; }
        public int baseValue { get; private set; }
        public int add { get; private set; }
        public int pctAdd { get; private set; }
        public int finalAdd { get; private set; }
        public int finalPctAdd { get; private set; }

        public void Initialize()
        {
            baseValue = add = pctAdd = finalAdd = finalPctAdd = 0;
        }
        public int SetBase(int value)
        {
            baseValue = value;
            Update();
            return baseValue;
        }
        public int Add(int value)
        {
            add += value;
            Update();
            return add;
        }
        public int PctAdd(int value)
        {
            pctAdd += value;
            Update();
            return pctAdd;
        }
        public int FinalAdd(int value)
        {
            finalAdd += value;
            Update();
            return finalAdd;
        }
        public int FinalPctAdd(int value)
        {
            finalPctAdd += value;
            Update();
            return finalPctAdd;
        }

        public void Update()
        {
            var value1 = baseValue;
            var value2 = (value1 + add) * (100 + pctAdd) / 100f;
            var value3 = (value2 + finalAdd) * (100 + finalPctAdd) / 100f;
            Value = (int)value3;
        }
    }

    /// <summary>
    /// 浮点型数值
    /// </summary>
    public class FloatNumeric
    {
        private float value;
        public float Value
        {
            get
            {
                Update();
                return value;
            }
        }
        public float Base { get; set; }
        public float Add { get; set; }
        public int   PctAdd { get; set; }
        public float FinalAdd { get; set; }
        public int   FinalPctAdd { get; set; }

        public void Update()
        {
            var value1 = Base;
            var value2 = (value1 + Add) * (100 + PctAdd) / 100f;
            var value3 = (value2 + FinalAdd) * (100 + FinalPctAdd) / 100f;
            value = value3;
        }
    }

    /// <summary>
    /// 战斗数值匣子，在这里管理所有角色战斗数值的存储、变更、刷新等
    /// </summary>
    public class CombatNumericBox
	{
		//public Dictionary<int, NumericItem> NumericItems = new Dictionary<int, NumericItem>();
		//public Dictionary<int, int> IntNumericBox = new Dictionary<int, int>();
        //public Dictionary<int, float> FloatNumericBox = new Dictionary<int, float>();
        public IntNumeric PhysicAttack_I = new IntNumeric();
        public IntNumeric PhysicDefense_I = new IntNumeric();
        public FloatNumeric CriticalProb_F = new FloatNumeric();


        public void Initialize()
		{
            // 这里初始化base值
            PhysicAttack_I.SetBase(1000);
            PhysicDefense_I.SetBase(300);
            CriticalProb_F.Base = 0.5f;
        }

        //public int Get(IntNumericType numericType)
        //{
        //    return IntNumericBox[(int)numericType];
        //}

        //public int GetBase(IntNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 1;
        //    return IntNumericBox[bas];
        //}

        //public int GetAdd(IntNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 2;
        //    return IntNumericBox[bas];
        //}

        //public float GetPct(IntNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 3;
        //    return IntNumericBox[bas] / 10000f;
        //}

        //public int GetFinalAdd(IntNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 4;
        //    return IntNumericBox[bas];
        //}

        //public int GetFinalPct(IntNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 5;
        //    return IntNumericBox[bas];
        //}

        //#region Float
        //public float Get(FloatNumericType numericType)
        //{
        //    return FloatNumericBox[(int)numericType];
        //}

        //public float GetBase(FloatNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 1;
        //    return FloatNumericBox[bas];
        //}

        //public float GetAdd(FloatNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 2;
        //    return FloatNumericBox[bas];
        //}

        //public float GetPct(FloatNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 3;
        //    return FloatNumericBox[bas];
        //}

        //public float GetFinalAdd(FloatNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 4;
        //    return FloatNumericBox[bas];
        //}

        //public float GetFinalPct(FloatNumericType numericType)
        //{
        //    int bas = (int)numericType * 10 + 5;
        //    return FloatNumericBox[bas];
        //}
        //#endregion

        //public void Update(IntNumericType numericType)
		//{
		//	//int final = (int)numericType / 10;
		//	//int bas = final * 10 + 1; 
		//	//int add = final * 10 + 2;
		//	//int pct = final * 10 + 3;
		//	//int finalAdd = final * 10 + 4;
		//	//int finalPct = final * 10 + 5;

		//	// 一个数值可能会多种情况影响，比如速度,加个buff可能增加速度绝对值100，也有些buff增加10%速度，所以一个值可以由5个值进行控制其最终结果
		//	// final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
		//	int result = (int)(((this.GetBase(numericType) + this.GetAdd(numericType)) * (100 + this.GetPct(pct)) / 100f + this.GetByKey(finalAdd)) * (100 + this.GetAsFloat(finalPct)) / 100f * 10000);
		//	this.IntNumericBox[(int)numericType] = result;
		//}

		//public void Update(FloatNumericType numericType)
		//{
		//	//int final = (int)numericType / 10;
		//	//int bas = final * 10 + 1;
		//	//int add = final * 10 + 2;
		//	//int pct = final * 10 + 3;
		//	//int finalAdd = final * 10 + 4;
		//	//int finalPct = final * 10 + 5;

		//	// 一个数值可能会多种情况影响，比如速度,加个buff可能增加速度绝对值100，也有些buff增加10%速度，所以一个值可以由5个值进行控制其最终结果
		//	// final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
		//	int result = (int)(((this.GetByKey(bas) + this.GetByKey(add)) * (100 + this.GetAsFloat(pct)) / 100f + this.GetByKey(finalAdd)) * (100 + this.GetAsFloat(finalPct)) / 100f * 10000);
		//	this.FloatNumericBox[(int)numericType] = result;
		//}
	}
}

/*
    /// <summary>
    /// 整形数值
    /// </summary>
    public class IntNumeric<T> where T : struct, ope
    {
        public T Value { get; private set; }
        public T baseValue { get; private set; }
        public T add { get; private set; }
        public int pctAdd { get; private set; }
        public T finalAdd { get; private set; }
        public int finalPctAdd { get; private set; }

        public void Initialize()
        {
            baseValue = add = pctAdd = finalAdd = finalPctAdd = default(T);
        }
        public T SetBase(T value)
        {
            baseValue = value;
            Update();
            return baseValue;
        }
        public int Add(int value)
        {
            add += value;
            Update();
            return add;
        }
        public int PctAdd(int value)
        {
            pctAdd += value;
            Update();
            return pctAdd;
        }
        public int FinalAdd(int value)
        {
            finalAdd += value;
            Update();
            return finalAdd;
        }
        public int FinalPctAdd(int value)
        {
            finalPctAdd += value;
            Update();
            return finalPctAdd;
        }

        public void Update()
        {
            if (baseValue is int value1 && add is int addValue && finalAdd is int finalAddValue)
            {
                var value2 = (value1 + addValue) * (100 + pctAdd) / 100f;
                var value3 = (value2 + finalAddValue) * (100 + finalPctAdd) / 100f;
                Value = value3;
            }
            //var value1 = baseValue;
            //var value2 = (value1 + add) * (100 + pctAdd) / 100f;
            //var value3 = (value2 + finalAdd) * (100 + finalPctAdd) / 100f;
            //Value = (int)value3;
        }
    }
 */