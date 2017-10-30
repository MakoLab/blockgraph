﻿#if !NOSOCKET

namespace NBitcoin.Protocol.Behaviors
{
	/// <summary>
	/// Maintain connection to a given set of nodes
	/// </summary>
	internal class NodesGroupBehavior : NodeBehavior
	{
		internal NodesGroup _Parent;

		public NodesGroupBehavior(NodesGroup parent)
		{
			_Parent = parent;
		}

		private NodesGroupBehavior()
		{
		}

		protected override void AttachCore()
		{
			AttachedNode.StateChanged += AttachedNode_StateChanged;
		}

		protected override void DetachCore()
		{
			AttachedNode.StateChanged -= AttachedNode_StateChanged;
		}

		private void AttachedNode_StateChanged(Node node, NodeState oldState)
		{
			if (node.State == NodeState.HandShaked)
			{
				_Parent._ConnectedNodes.Add(node);
			}
			if (node.State == NodeState.Failed || node.State == NodeState.Disconnecting || node.State == NodeState.Offline)
			{
				if (_Parent._ConnectedNodes.Remove(node))
				{
					_Parent.StartConnecting();
				}
			}
		}

		#region ICloneable Members

		public override object Clone()
		{
			return new NodesGroupBehavior(_Parent);
		}

		#endregion ICloneable Members
	}
}

#endif