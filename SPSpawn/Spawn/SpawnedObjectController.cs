﻿using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    public class SpawnedObjectController : SPComponent, IKillableEntity
    {

        #region Fields

        [System.NonSerialized()]
        private SpawnPool _pool;
        [System.NonSerialized]
        private int _prefabId;
        [System.NonSerialized()]
        private string _sCacheName;
        
        [System.NonSerialized()]
        private bool _isSpawned;

        #endregion

        #region CONSTRUCTOR

        internal void Init(SpawnPool pool, int prefabId)
        {
            _pool = pool;
            _prefabId = prefabId;
            _sCacheName = null;
        }

        /// <summary>
        /// Initialize with a reference to the pool that spawned this object. Include a cache name if this gameobject is cached, otherwise no cache name should be included.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="prefab">prefab this was spawned from</param>
        /// <param name="sCacheName"></param>
        internal void Init(SpawnPool pool, int prefabId, string sCacheName)
        {
            _pool = pool;
            _prefabId = prefabId;
            _sCacheName = sCacheName;
        }

        internal void DeInit()
        {
            _pool = null;
            _prefabId = 0;
            _sCacheName = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(!GameLoop.ApplicationClosing && _pool != null)
            {
                _pool.Purge(this);
            }
        }

        #endregion

        #region Properties

        public bool IsSpawned
        {
            get { return _isSpawned; }
        }

        /// <summary>
        /// The pool that created this object.
        /// </summary>
        public SpawnPool Pool
        {
            get { return _pool; }
        }

        public int PrefabID
        {
            get { return _prefabId; }
        }
        
        public string CacheName
        {
            get { return _sCacheName; }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// This method ONLY called by SpawnPool
        /// </summary>
        internal void SetSpawned()
        {
            _isSpawned = true;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// This method ONLY called by SpawnPool
        /// </summary>
        internal void SetDespawned()
        {
            _isSpawned = false;
            this.gameObject.SetActive(false);
        }

        public GameObject CloneObject(bool fromPrefab = false)
        {
            //TODO
            return null;
        }

        #endregion

        #region IKillableEntity Interface

        public bool IsDead
        {
            get { return !_isSpawned; }
        }

        public void Kill()
        {
            if (!_pool.Despawn(this))
            {
                ObjUtil.SmartDestroy(this.gameObject);
            }
            else
            {
                //TODO - need a cleaner way of doing this
                using (var lst = TempCollection.GetList<Rigidbody>())
                {
                    this.transform.GetComponentsInChildren<Rigidbody>(lst);
                    var e = lst.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.velocity = Vector3.zero;
                        e.Current.angularVelocity = Vector3.zero;
                    }
                }
                using (var lst = TempCollection.GetList<Rigidbody2D>())
                {
                    this.transform.GetComponentsInChildren<Rigidbody2D>(lst);
                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        e.Current.velocity = Vector2.zero;
                        e.Current.angularVelocity = 0f;
                    }
                }
            }
            
        }

            #endregion

        }

}
