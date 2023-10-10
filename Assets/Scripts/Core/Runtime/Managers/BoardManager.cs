using BonLib.Managers;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class BoardManager : Manager<BoardManager>
    {
        private BoardConfig m_config;
        public BoardConfig Config => m_config ??= Resources.Load<BoardConfig>("Config/BoardConfig");

        public override void PreInitialize()
        {
            base.PreInitialize();

            var drawSlotsEvt = new DrawSlotsEvent(Config.Dimensions);
            EventManager.SendEvent(ref drawSlotsEvt);
        }
    }

}