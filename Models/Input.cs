using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slime.Models
{
    public class Input
    {
        MouseState mState = Mouse.GetState();
        public Keys moveUp { get; set; }
        public Keys moveDown { get; set; }
        public Keys moveLeft { get; set; }
        public Keys moveRight { get; set; }
        public Keys teleport { get; set; }
        public Keys teleportHome { get; set; }
        public Keys stopTime { get; set; }
        public Keys startTime { get; set; }
    }
}
