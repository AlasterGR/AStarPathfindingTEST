using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// Sets the destination of an AI to the position of a specified object, taken from a list of nodes.
    /// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
    /// This component will then make the AI move towards the <see cref="target"/> set on this component.
    /// </summary>
    public class MoveToTarget : VersionedMonoBehaviour
    {
        /// <summary>The list of objects that the AI should move to</summary>
		[Tooltip("The parent object whose children are the nodes that the AI will move to. Can be set to a single object as well as the player.")]
        [SerializeField] Transform targetRoot;

        //  Can be safely turned into private
        [Tooltip("The list of all possible targets, the shildren of targetRoot.")]
        [SerializeField] private Transform[] targets;

        [SerializeField] bool PickTargetRandomlyFromTargets = false;
        /// <summary>Time in seconds to wait at each target</summary>
        [Tooltip("Time in seconds to wait at each target.")]
        [SerializeField] float delay = 0;
        /// <summary>Current target index</summary>
        [Tooltip("Current target index.")]
        [SerializeField] int index = 0;  // 0 index is the parent node and that would make the process freeze. No worries, it changes later before it is used.
        /// <summary>The NPC / AI agent</summary>
        IAstarAI agent;
		float switchTime = float.PositiveInfinity;
        /* The agent's current destination. Can be safely deleted */
        [SerializeField] Transform Destination ;  

        protected override void Awake()
		{
			base.Awake();
			agent = GetComponent<IAstarAI>();
			targets = targetRoot.GetComponentsInChildren<Transform>();
			if (PickTargetRandomlyFromTargets == true) { index = UnityEngine.Random.Range(1, targets.Length) ; }
            else { index = index + 1; }
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
			{	switchTime = Time.time + delay;	}

			if (Time.time >= switchTime)
			{
                if (PickTargetRandomlyFromTargets == true) { index = UnityEngine.Random.Range(1, targets.Length); }
                else { index = index + 1; }
                //index = UnityEngine.Random.Range(1, targets.Length);  // from 1 and not 0, because 0 is the parent node and makes the process to freeze
                //Debug.Log("index = " + index);
                search = true;
				switchTime = float.PositiveInfinity;
			}

			index = index % targets.Length;
            if (index == 0) { index = 1; }  // We make sure that index is not 0
            agent.destination = targets[index].position;
			Destination = targets[index];  // The agent's current destination node, from the targets list. Can be safely deleted

            if (search) agent.SearchPath();
		}
	}
}
