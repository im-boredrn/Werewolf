using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace WereWolf.assets.Coresystems
{
    internal class WolfTime
    {
        public static bool isNight(Entity entity)
        {
            var hour = entity?.World?.Calendar.HourOfDay ?? 0; // fallback in case entity/world is null
            return (hour >= 18 || hour <= 6);


        }

      
            }
        }


    
