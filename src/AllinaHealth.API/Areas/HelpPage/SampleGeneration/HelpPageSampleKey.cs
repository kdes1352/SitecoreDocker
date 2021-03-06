using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;

namespace AllinaHealth.WebAPI.Areas.HelpPage.SampleGeneration
{
    /// <summary>
    /// This is used to identify the place where the sample should be applied.
    /// </summary>
    public class HelpPageSampleKey
    {
        /// <summary>
        /// Creates a new <see cref="HelpPageSampleKey"/> based on media type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        public HelpPageSampleKey(MediaTypeHeaderValue mediaType)
        {
            ActionName = string.Empty;
            ControllerName = string.Empty;
            MediaType = mediaType ?? throw new ArgumentNullException("mediaType");
            ParameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a new <see cref="HelpPageSampleKey"/> based on media type and CLR type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="type">The CLR type.</param>
        public HelpPageSampleKey(MediaTypeHeaderValue mediaType, Type type)
            : this(mediaType)
        {
            ParameterType = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Creates a new <see cref="HelpPageSampleKey"/> based on <see cref="SampleDirection"/>, controller name, action name and parameter names.
        /// </summary>
        /// <param name="sampleDirection">The <see cref="SampleDirection"/>.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameterNames">The parameter names.</param>
        public HelpPageSampleKey(SampleDirection sampleDirection, string controllerName, string actionName, IEnumerable<string> parameterNames)
        {
            if (!Enum.IsDefined(typeof(SampleDirection), sampleDirection))
            {
                throw new InvalidEnumArgumentException(nameof(sampleDirection), (int)sampleDirection, typeof(SampleDirection));
            }

            if (parameterNames == null)
            {
                throw new ArgumentNullException(nameof(parameterNames));
            }

            ControllerName = controllerName ?? throw new ArgumentNullException(nameof(controllerName));
            ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
            ParameterNames = new HashSet<string>(parameterNames, StringComparer.OrdinalIgnoreCase);
            SampleDirection = sampleDirection;
        }

        /// <summary>
        /// Creates a new <see cref="HelpPageSampleKey"/> based on media type, <see cref="SampleDirection"/>, controller name, action name and parameter names.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="sampleDirection">The <see cref="SampleDirection"/>.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameterNames">The parameter names.</param>
        public HelpPageSampleKey(MediaTypeHeaderValue mediaType, SampleDirection sampleDirection, string controllerName, string actionName, IEnumerable<string> parameterNames)
            : this(sampleDirection, controllerName, actionName, parameterNames)
        {
            MediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
        }

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <value>
        /// The name of the controller.
        /// </value>
        public string ControllerName { get; }

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        public string ActionName { get; }

        /// <summary>
        /// Gets the media type.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        public MediaTypeHeaderValue MediaType { get; }

        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        public HashSet<string> ParameterNames { get; }

        public Type ParameterType { get; }

        /// <summary>
        /// Gets the <see cref="SampleDirection"/>.
        /// </summary>
        public SampleDirection? SampleDirection { get; }

        public override bool Equals(object obj)
        {
            var otherKey = obj as HelpPageSampleKey;
            if (otherKey == null)
            {
                return false;
            }

            return string.Equals(ControllerName, otherKey.ControllerName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(ActionName, otherKey.ActionName, StringComparison.OrdinalIgnoreCase) &&
                   (MediaType == otherKey.MediaType || (MediaType != null && MediaType.Equals(otherKey.MediaType))) &&
                   ParameterType == otherKey.ParameterType &&
                   SampleDirection == otherKey.SampleDirection &&
                   ParameterNames.SetEquals(otherKey.ParameterNames);
        }

        public override int GetHashCode()
        {
            var hashCode = ControllerName.ToUpperInvariant().GetHashCode() ^ ActionName.ToUpperInvariant().GetHashCode();
            if (MediaType != null)
            {
                hashCode ^= MediaType.GetHashCode();
            }

            if (SampleDirection != null)
            {
                hashCode ^= SampleDirection.GetHashCode();
            }

            if (ParameterType != null)
            {
                hashCode ^= ParameterType.GetHashCode();
            }

            return ParameterNames.Aggregate(hashCode, (current, parameterName) => current ^ parameterName.ToUpperInvariant().GetHashCode());
        }
    }
}