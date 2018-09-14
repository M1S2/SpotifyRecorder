using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify_Recorder
{
    /// <summary>
    /// Fade types
    /// </summary>
    public enum FadeTypes
    {
        /// <summary>
        /// Linear fade
        /// </summary>
        LINEAR,

        /// <summary>
        /// Logarithmic fade
        /// </summary>
        LOG,

        /// <summary>
        /// Hyperbolic fade
        /// </summary>
        HYPERBEL,

        /// <summary>
        /// Custom fade
        /// </summary>
        CUSTOM,

        /// <summary>
        /// Undo linear fade
        /// </summary>
        UNDO_LINEAR,

        /// <summary>
        /// Undo logarithmic fade
        /// </summary>
        UNDO_LOG,

        /// <summary>
        /// Undo hyperbolic fade
        /// </summary>
        UNDO_HYPERBEL,

        /// <summary>
        /// Undo custom fade
        /// </summary>
        UNDO_CUSTOM
    }
}
