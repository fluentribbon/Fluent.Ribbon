// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fluent.Converters;

    /// <summary>
    /// This class holds the Holds transitionable states when the <see cref="RibbonGroupsContainer"/> automatically resizes the <see cref="RibbonGroupBox"/>.
    /// </summary>
    [TypeConverter(typeof(StateDefinitionConverter))]
    public struct RibbonGroupBoxStateDefinition : IEquatable<RibbonGroupBoxStateDefinition>
    {
        private const int MaxStateDefinitionParts = 4;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public RibbonGroupBoxStateDefinition(string? stateDefinition)
            : this()
        {
            this.states = new List<RibbonGroupBoxState>
                {
                    RibbonGroupBoxState.Large,
                    RibbonGroupBoxState.Middle,
                    RibbonGroupBoxState.Small,
                    RibbonGroupBoxState.Collapsed,
                };

            if (string.IsNullOrEmpty(stateDefinition))
            {
                return;
            }

            var splitted = stateDefinition!.Split(new[] { ' ', ',', ';', '-', '>' }, MaxStateDefinitionParts, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (splitted.Count == 0)
            {
                return;
            }

            var states = new List<RibbonGroupBoxState>();
            foreach (var item in splitted)
            {
                var state = ToRibbonGroupBoxState(item);
                if (!states.Contains(state))
                {
                    if (state != RibbonGroupBoxState.QuickAccess)
                    {
                        states.Add(state);
                    }
                }

                if (states.Count >= MaxStateDefinitionParts)
                {
                    break;
                }
            }

            if (states.Count > 0)
            {
                states.Sort();  // sort large to small
                this.states = states;
            }
        }

        /// <summary>
        /// Gets or sets the transitionable states
        /// </summary>
        /// 
        public IReadOnlyList<RibbonGroupBoxState> States
        {
            get { return this.states; }
        }

        private readonly List<RibbonGroupBoxState> states;

        /// <summary>
        /// Converts from <see cref="string"/> to <see cref="RibbonGroupBoxStateDefinition"/>
        /// </summary>
        public static RibbonGroupBoxStateDefinition FromString(string stateDefinition)
        {
            return new RibbonGroupBoxStateDefinition(stateDefinition);
        }

        /// <summary>
        /// Converts from <see cref="string"/> to <see cref="RibbonGroupBoxStateDefinition"/>
        /// </summary>
        public static implicit operator RibbonGroupBoxStateDefinition(string stateDefinition)
        {
            return FromString(stateDefinition);
        }

        /// <summary>
        /// Converts from <see cref="RibbonGroupBoxStateDefinition"/> to <see cref="string"/>
        /// </summary>
        public static implicit operator string(RibbonGroupBoxStateDefinition stateDefinition)
        {
            return stateDefinition.ToString();
        }

        /// <summary>
        /// Converts from <see cref="string"/> to <see cref="RibbonGroupBoxState"/>
        /// </summary>
        public static RibbonGroupBoxState ToRibbonGroupBoxState(string ribbonControlState)
        {
            return Enum.TryParse(ribbonControlState, true, out RibbonGroupBoxState result)
                       ? result
                       : RibbonGroupBoxState.Large;
        }

        /// <summary>
        /// Gets the appropriate enlarged <see cref="RibbonGroupBoxState"/> depending on StateDefinition />
        /// </summary>
        public RibbonGroupBoxState EnlargeState(RibbonGroupBoxState ribbonGroupBoxState)
        {
            var index = this.states.IndexOf(ribbonGroupBoxState);
            if (index >= 0)
            {
                if (index > 0)
                {
                    return this.states[index - 1];
                }
                else
                {
                    return this.states.First();
                }
            }
            else
            {
                //  If not found current state, find the closest state that exists in the state list
                while (--ribbonGroupBoxState >= RibbonGroupBoxState.Large)
                {
                    index = this.states.IndexOf(ribbonGroupBoxState);
                    if (index >= 0)
                    {
                        return this.states[index];
                    }
                }

                return this.states.First();
            }
        }

        /// <summary>
        /// Gets the appropriate reduced <see cref="RibbonGroupBoxState"/> depending on StateDefinition />
        /// </summary>
        public RibbonGroupBoxState ReduceState(RibbonGroupBoxState ribbonGroupBoxState)
        {
            var index = this.states.IndexOf(ribbonGroupBoxState);
            if (index >= 0)
            {
                if (index < this.states.Count - 1)
                {
                    return this.states[index + 1];
                }
                else
                {
                    return this.states.Last();
                }
            }
            else
            {
                // If not found current state, find the closest state that exists in the state list
                while (++ribbonGroupBoxState <= RibbonGroupBoxState.Collapsed)
                {
                    index = this.states.IndexOf(ribbonGroupBoxState);
                    if (index >= 0)
                    {
                        return this.states[index];
                    }
                }

                return this.states.Last();
            }
        }

        #region Overrides of ValueType

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is RibbonGroupBoxStateDefinition definition
                   && this.Equals(definition);
        }

        #region Equality members

        /// <inheritdoc />
        public bool Equals(RibbonGroupBoxStateDefinition other)
        {
            if (this.states.Count != other.states.Count)
            {
                return false;
            }

            for (int i = 0; i < this.states.Count; i++)
            {
                if (this.states[i] != other.states[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)this.states.Count;
                this.states.ForEach(i => hashCode = (hashCode * 397) ^ (int)i);
                return hashCode;
            }
        }

        /// <summary>Determines whether the specified object instances are considered equal.</summary>
        /// <param name="left">The first object to compare. </param>
        /// <param name="right">The second object to compare. </param>
        /// <returns>true if the objects are considered equal; otherwise, false. If both <paramref name="left" /> and <paramref name="right" /> are null, the method returns true.</returns>
        public static bool operator ==(RibbonGroupBoxStateDefinition left, RibbonGroupBoxStateDefinition right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether the specified object instances are not considered equal.</summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the objects are not considered equal; otherwise, false. If both <paramref name="left" /> and <paramref name="right" /> are null, the method returns false.</returns>
        public static bool operator !=(RibbonGroupBoxStateDefinition left, RibbonGroupBoxStateDefinition right)
        {
            return !left.Equals(right);
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Join(",", this.states);
        }
    }
}
