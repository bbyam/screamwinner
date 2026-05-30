using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreamControl
{
    internal interface IAnimation
    {
        void OnAnimate();
        void ReceiveEvent(ScreamEvents screamEvent);
        bool IsDead();
    }
}
