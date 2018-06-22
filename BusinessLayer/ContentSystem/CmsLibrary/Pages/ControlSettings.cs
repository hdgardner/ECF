using System;

namespace Mediachase.Cms.Pages
{
    /// <summary>
    /// Summary description for ControlSettings
    /// </summary>
    [Serializable]
    public class ControlSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlSettings"/> class.
        /// </summary>
        public ControlSettings()
        {
        }

        private Param _params;

        private bool _isModified = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = value;
            }
        }

        /// <summary>
        /// Gets or sets the params.
        /// </summary>
        /// <value>The params.</value>
        public Param Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }
    }
}