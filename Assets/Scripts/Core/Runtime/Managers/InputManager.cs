using System;
using BonLib.DependencyInjection;
using BonLib.Managers;
using Core.Config;
using Core.Misc;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Helpers;
using Core.Runtime.Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Runtime.Managers
{

    public class InputManager : Manager<InputManager>,
        IPointerDownHandler, 
        IDragHandler
    {
        private InputConfig m_config;
        public InputConfig Config => m_config ??= Resources.Load<InputConfig>("Config/InputConfig");

        private Camera m_camera;
        public Camera Camera => m_camera ??= Camera.main;

        private BoardManager m_boardManager;

        private Vector2Int m_boardDimensions;
        
        private Vector3 m_dragStartPos;

        private bool m_isDragging;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_boardManager = DI.Resolve<BoardManager>();

            m_boardDimensions = m_boardManager.Config.Dimensions;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_isDragging = false;
            m_dragStartPos = Vector3.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_dragStartPos = Camera.ScreenToWorldPoint(eventData.position);
            m_isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!m_isDragging)
                return;

            var dragPos = Camera.ScreenToWorldPoint(eventData.position);

            var diff = dragPos - m_dragStartPos;

            if (Vector2.SqrMagnitude(diff) < Config.SwipeSensitivity)
                return;
            
            m_isDragging = false;
            
            var direction = Direction.NULL;

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                direction = diff.x > 0 ? Direction.RIGHT : Direction.LEFT;
            }
            else
            {
                direction = diff.y > 0 ? Direction.UP : Direction.DOWN;
            }

            var dragStart = m_boardManager.GetIndexFromWorldPos(m_dragStartPos);

            int dragEnd;

            var isValidSwipe = true;
            
            switch (direction)
            {
                case Direction.LEFT:
                {
                    if (dragStart.GetCoordinates(m_boardDimensions.x).x == 0)
                    {
                        isValidSwipe = false;
                    }

                    dragEnd = dragStart - 1;
                }
                    break;
                case Direction.RIGHT:
                {
                    if (dragStart.GetCoordinates(m_boardDimensions.x).x == m_boardDimensions.x - 1)
                    {
                        isValidSwipe = false;
                    }

                    dragEnd = dragStart + 1;
                }
                    break;
                case Direction.UP:
                {
                    if (dragStart.GetCoordinates(m_boardDimensions.y).y == m_boardDimensions.y - 1)
                    {
                        isValidSwipe = false;
                    }

                    dragEnd = dragStart + m_boardDimensions.x;
                }
                    break;
                case Direction.DOWN:
                {
                    if (dragStart.GetCoordinates(m_boardDimensions.y).y == 0)
                    {
                        isValidSwipe = false;
                    }
                    
                    dragEnd = dragStart - m_boardDimensions.x;
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!isValidSwipe)
                return;
            
            var evt = new SwapSlotsEvent(new Slot(dragStart), new Slot(dragEnd), Config.SwipeDuration);
            EventManager.SendEvent(ref evt);
                
            Debug.LogWarning($"Swipe from {dragStart} to {dragEnd}");
        }
    }

}