using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Helper;

public sealed class WaitLock<T>
{
    public T Value;

    public WaitLock(T value)
    {
        Value = value;
    }
}
