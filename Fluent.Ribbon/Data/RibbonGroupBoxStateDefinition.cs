// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Fluent.Converters;

    /// <summary>
    /// This class holds the Holds transitionable states when the <see cref="RibbonGroupsContainer"/> automatically resizes the <see cref="RibbonGroupBox"/>.
    /// </summary>
    [TypeConverter(typeof(RibbonGroupBoxStateDefinitionConverter))]
    public readonly struct RibbonGroupBoxStateDefinition : IEquatable<RibbonGroupBoxStateDefinition>
    {
        private static readonly RibbonGroupBoxState[] defaultStates =
        {
            RibbonGroupBoxState.Large,
            RibbonGroupBoxState.Middle,
            RibbonGroupBoxState.Small,
            RibbonGroupBoxState.Collapsed,
        };

        private const int MaxStateDefinitionParts = 4;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public RibbonGroupBoxStateDefinition(string? stateDefinition)
            : this()
        {
            this.states = defaultStates;

            if (string.IsNullOrEmpty(stateDefinition))
            {
                return;
            }

            var splitted = stateDefinition!.Split(new[] { ' ', ',', ';', '-', '>' }, MaxStateDefinitionParts, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length == 0)
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
                this.states = states.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the transitionable states
        /// </summary>
        public IReadOnlyList<RibbonGroupBoxState> States => this.GetStates();

        private readonly RibbonGroupBoxState[]? states;

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
            var currentStates = this.GetStates();
            var index = Array.IndexOf(currentStates, ribbonGroupBoxState);
            if (index >= 0)
            {
                if (index > 0)
                {
                    return this.States[index - 1];
                }
                else
                {
                    return this.States[0];
                }
            }
            else
            {
                //  If not found current state, find the closest state that exists in the state list
                while (--ribbonGroupBoxState >= RibbonGroupBoxState.Large)
                {
                    index = Array.IndexOf(currentStates, ribbonGroupBoxState);
                    if (index >= 0)
                    {
                        return this.States[index];
                    }
                }

                return this.States[0];
            }
        }

        /// <summary>
        /// Gets the appropriate reduced <see cref="RibbonGroupBoxState"/> depending on StateDefinition />
        /// </summary>
        public RibbonGroupBoxState ReduceState(RibbonGroupBoxState ribbonGroupBoxState)
        {
            var currentStates = this.GetStates();
            var index = Array.IndexOf(currentStates, ribbonGroupBoxState);
            if (index >= 0)
            {
                if (index < currentStates.Length - 1)
                {
                    return currentStates[index + 1];
                }
                else
                {
                    return currentStates[currentStates.Length - 1];
                }
            }
            else
            {
                // If not found current state, find the closest state that exists in the state list
                while (++ribbonGroupBoxState <= RibbonGroupBoxState.Collapsed)
                {
                    index = Array.IndexOf(currentStates, ribbonGroupBoxState);
                    if (index >= 0)
                    {
                        return currentStates[index];
                    }
                }

                return currentStates[currentStates.Length - 1];
            }
        }

        private RibbonGroupBoxState[] GetStates()
        {
            return this.states ?? defaultStates;
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
            var currentStates = this.GetStates();
            if (currentStates.Length != other.States.Count)
            {
                return false;
            }

            for (var i = 0; i < currentStates.Length; i++)
            {
                if (currentStates[i] != other.States[i])
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
                var currentStates = this.GetStates();
                var hashCode = currentStates.Length;
                foreach (var state in currentStates)
                {
                    hashCode = (hashCode * 397) ^ (int)state;
                }

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
            return string.Join(",", this.States);
        }
    }
}
