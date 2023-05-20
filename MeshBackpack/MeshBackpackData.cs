using System;
using System.Collections.Generic;
using UnityEngine;

namespace CagrsLib.MeshBackpack
{
    [Serializable]
    public class ItemContainer <T>
    {
        private int _containerWidth;
        private int _containerHeight;

        private ItemRotation _rotation = ItemRotation.Horizontal;

        private T _itemObject;

        public ItemContainer(int width, int height, T obj)
        {
            _itemObject = obj;
            _containerWidth = width;
            _containerHeight = height;
        }

        #region Rotation

        public void Rotate()
        {
            if (_rotation == ItemRotation.Horizontal) _rotation = ItemRotation.Vertical;
            if (_rotation == ItemRotation.Vertical) _rotation = ItemRotation.Horizontal;
        }

        public void Rotate(ItemRotation rotation)
        {
            _rotation = rotation;
        }

        public ItemSize GetItemSize()
        {
            if (_rotation == ItemRotation.Horizontal)
            {
                return new ItemSize
                {
                    width = _containerWidth,
                    height = _containerHeight
                };
            }
            else
            {
                return new ItemSize
                {
                    width = _containerHeight,
                    height = _containerWidth
                };
            }
        }

        #endregion

        [Serializable]
        public enum ItemRotation
        {
            Horizontal, Vertical
        }
        
        [Serializable]
        public struct ItemSize
        {
            public int width, height;
        }
    }

    [Serializable]
    public class BackpackData <T>
    {
        private int _backpackWidth;
        private int _backpackHeight;

        private List<List<SlotState>> _slotStates = new();

        private Dictionary<BackPackPos, ItemContainer<T>> _itemContainers = new();

        public BackpackData(int width, int height)
        {
            _backpackWidth = width;
            _backpackHeight = height;

            ReBuildSlotStates();
        }

        #region Build

        public void ReBuildSlotStates()
        {
            for (int h = 0; h <= _backpackHeight; h ++)
            {
                List<SlotState> line = new List<SlotState>();
                
                _slotStates.Add(line);
                
                for (int w = 0; w <= _backpackWidth; w ++)
                {
                    line.Add(SlotState.Unavailable);
                }
            }
        }

        #endregion
        
        #region Item Put & Get

        public bool TryPutItem(int x, int y, ItemContainer<T> itemContainer)
        {
            if (itemContainer == null) return false;

            ItemContainer<T>.ItemSize itemSize = itemContainer.GetItemSize();

            if (IsInSlotRange(x, y, itemSize)) return false;

            if (SlotCheck(x, y, itemSize))
            {
                _itemContainers.Add(
                    new BackPackPos { x = x, y = y }, 
                    itemContainer);
                
                SignSlotUnavailable(x, y, itemContainer);
            }
            
            return true;
        }

        public bool TryPutItem(BackPackPos pos, ItemContainer<T> itemContainer)
        {
            return TryPutItem(pos.x, pos.y, itemContainer);
        }

        public bool TryDropItem(ItemContainer<T> itemContainer)
        {
            if (itemContainer == null) return false;

            if (!_itemContainers.ContainsValue(itemContainer)) return false;

            foreach (var container in _itemContainers)
            {
                if (container.Value.Equals(itemContainer))
                {
                    _itemContainers.Remove(container.Key);
                    
                    SignSlotAvailable(container.Key, itemContainer);
                    return true;
                }
            }

            return false;
        }

        public bool TryDropItem(BackPackPos pos, out ItemContainer<T> container)
        {
            container = null;
            
            if (!_itemContainers.ContainsKey(pos)) return false;
            
            container = _itemContainers[pos];

            if (container == null) return false;
            
            if (!IsInSlotRange(pos, container.GetItemSize())) return false;

            _itemContainers.Remove(pos);
                    
            SignSlotAvailable(pos, container);
            
            return true;
        }
        
        public bool TryDropItem(int x, int y, out ItemContainer<T> container)
        {
            bool result = TryDropItem(new BackPackPos { x = x, y = y }, out ItemContainer<T> output);
            container = output;
            return result;
        }

        public bool TryDropItem(BackPackPos pos)
        {
            bool result = TryDropItem(pos, out ItemContainer<T> output);
            return result;
        }
        
        public bool TryDropItem(int x, int y)
        {
            bool result = TryDropItem(new BackPackPos { x = x, y = y }, out ItemContainer<T> output);
            return result;
        }

        #endregion

        #region Slot Set & Get & Put & Clear & Check

        public SlotState GetSlotState(int x, int y)
        {
            return _slotStates[y][x];
        }
        
        public void SetSlotState(int x, int y, SlotState state)
        {
            _slotStates[y][x] = state;
        }
        
        public void PutSlotState(int x, int y)
        { 
            _slotStates[y][x] = SlotState.Available;
        }
        
        public void ClearSlotState(int x, int y)
        { 
            _slotStates[y][x] = SlotState.Unavailable;
        }
        
        public SlotState GetSlotState(BackPackPos pos)
        {
            return GetSlotState(pos.x, pos.y);
        }
        
        public void SetSlotState(BackPackPos pos, SlotState state)
        {
            SetSlotState(pos.x, pos.y, state);
        }
        
        public void PutSlotState(BackPackPos pos)
        { 
            PutSlotState(pos.x, pos.y);
        }
        
        public void ClearSlotState(BackPackPos pos)
        { 
            ClearSlotState(pos.x, pos.y);
        }

        public bool SlotCheck(BackPackPos pos, ItemContainer<T>.ItemSize size)
        {
            for (int w = 0; w < size.width; w++)
            {
                for (int h = 0; h < size.height; h++)
                {
                    if (GetSlotState(pos.x + w, pos.y + h) == SlotState.Unavailable) return false;
                }
            }

            return true;
        }

        public bool SlotCheck(int x, int y, ItemContainer<T>.ItemSize size)
        {
            return SlotCheck(new BackPackPos{x = x, y = y}, size);
        }

        public bool IsInSlotRange(BackPackPos pos, ItemContainer<T>.ItemSize size)
        {
            return pos.x + size.width <= _backpackWidth && pos.y + size.height <= _backpackHeight;
        }

        public bool IsInSlotRange(int x, int y, ItemContainer<T>.ItemSize size)
        {
            return IsInSlotRange(new BackPackPos{x = x, y = y}, size);
        }

        public void SignSlotUnavailable(int x, int y, ItemContainer<T> itemContainer)
        {
            SignSlotUnavailable(new BackPackPos{x = x, y = y}, itemContainer);
        }
        
        public void SignSlotAvailable(int x, int y, ItemContainer<T> itemContainer)
        {
            SignSlotAvailable(new BackPackPos{x = x, y = y}, itemContainer);
        }

        public void SignSlotUnavailable(BackPackPos pos, ItemContainer<T> itemContainer)
        {
            ItemContainer<T>.ItemSize size = itemContainer.GetItemSize();
            for (int w = 0; w < size.width; w++)
            {
                for (int h = 0; h < size.height; h++)
                {
                    PutSlotState(pos.x + w, pos.y + h);
                }
            }
        }
        
        //TODO:: 和上面那家伙一起显得有点冗余了，得治
        public void SignSlotAvailable(BackPackPos pos, ItemContainer<T> itemContainer)
        {
            ItemContainer<T>.ItemSize size = itemContainer.GetItemSize();
            for (int w = 0; w < size.width; w++)
            {
                for (int h = 0; h < size.height; h++)
                {
                    ClearSlotState(pos.x + w, pos.y + h);
                }
            }
        }

        #endregion

        [Serializable]
        public enum SlotState
        {
            Available,  Unavailable
        }
        
        [Serializable]
        public struct BackPackPos
        {
            public int x, y;
        }
    }
}