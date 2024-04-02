using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class MoveToRandomTarget : VersionedMonoBehaviour
    {
		/// <summary>Target points to move to</summary>
		public Transform targetRoot;
        private Transform[] targets;  //  The list of possible targets for our AI, which is the set of targetRoot's children 
        
        /// <summary>Time in seconds to wait at each target</summary>
        public float delay = 0;

		/// <summary>Current target index</summary>
		int index;

		IAstarAI agent;
		float switchTime = float.PositiveInfinity;
        /* The agent's current destination. Can be safely deleted */
        public Transform Destination ;  

        protected override void Awake()
		{
			base.Awake();
			agent = GetComponent<IAstarAI>();
			targets = targetRoot.GetComponentsInChildren<Transform>();
			index = UnityEngine.Random.Range(1, targets.Length);  // from 1 and not 0, bvecause 0 is the parent node and makes the process to freeze
			//Debug.Log("targets.length = " + targets.Length);
			//Debug.Log("index = " + index);
		}

		/// <summary>Update is called once per frame</summary>
		void Update()
		{
			if (targets.Length == 0) return;

			bool search = false;

			// Note: using reachedEndOfPath and pathPending instead of reachedDestination here because
			// if the destination cannot be reached by the agent, we don't want it to get stuck, we just want it to get as close as possible and then move on.
			if (agent.reachedEndOfPath && !agent.pathPending && float.IsPositiveInfinity(switchTime))
			{
				switchTime = Time.time + delay;
			}

			if (Time.time >= switchTime)
			{
				index = UnityEngine.Random.Range(1, targets.Length);  // from 1 and not 0, because 0 is the parent node and makes the process to freeze
				//Debug.Log("index = " + index);
				search = true;
				switchTime = float.PositiveInfinity;
			}

			index = index % targets.Length;
			agent.destination = targets[index].position;
			Destination = targets[index];  // The agent's current destination node, from the targets list. Can be safely deleted

            if (search) agent.SearchPath();
		}
	}
}
