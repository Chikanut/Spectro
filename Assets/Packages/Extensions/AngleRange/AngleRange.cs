using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AngleRange
{
    public interface IAnleRangeUser
    {
        List<AngleRange> AngleRanges { get; set; }
    }

    [System.Serializable]
    public class AngleRange
    {
        public float start
        {
            get { return m_Start; }
            set { m_Start = value; }
        }

        public float end
        {
            get { return m_End; }
            set { m_End = value; }
        }

        public int order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }


        [SerializeField] float m_Start;
        [SerializeField] float m_End;
        [SerializeField] int m_Order;

        public object Clone()
        {
            AngleRange clone = this.MemberwiseClone() as AngleRange;

            return clone;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AngleRange;

            if (other == null)
                return false;

            bool equals = start.Equals(other.start) && end.Equals(other.end) && order.Equals(other.order);

            if (!equals)
                return false;

            return true;
        }
    }
}
