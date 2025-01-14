﻿using System;
using Melia.Shared.World;

namespace Melia.Channel.World.Entities.Components.AI.Routines
{
	/// <summary>
	/// A routine that makes the monster walk to a position.
	/// </summary>
	public class MoveToRoutine : IRoutine
	{
		private EntityAi _ai;
		private Position _destination;
		private bool _started;
		private TimeSpan _moveTime;

		/// <summary>
		/// Creates routine.
		/// </summary>
		/// <param name="ai"></param>
		/// <param name="distance"></param>
		public MoveToRoutine(EntityAi ai, Position destination)
		{
			this.Init(ai, destination);
		}

		/// <summary>
		/// Initializes routine.
		/// </summary>
		/// <param name="ai"></param>
		/// <param name="distance"></param>
		public void Init(EntityAi ai, Position destination)
		{
			_ai = ai;
			_destination = destination;
			_started = false;
		}

		/// <summary>
		/// Executes routine, returns Success once the monster has reached
		/// the random destination.
		/// </summary>
		/// <param name="elapsed"></param>
		/// <returns></returns>
		public RoutineResult Execute(TimeSpan elapsed)
		{
			if (_started)
			{
				_moveTime -= elapsed;

				if (_moveTime > TimeSpan.Zero)
					return RoutineResult.Running;

				return RoutineResult.Success;
			}

			if (_ai.Entity.Components.TryGet<Movement>(out var movement))
			{
				_moveTime = movement.MoveTo(_destination);
				_started = true;

				return RoutineResult.Running;
			}

			return RoutineResult.Fail;
		}
	}
}
