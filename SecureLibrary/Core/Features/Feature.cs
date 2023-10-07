using Material.Icons;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core.Features
{
    /// <summary>
    /// Base feature class that is necessary for easy modification a Secure Library application.
    /// All Feature instances with RegisterFeature attribute will be collected at startup and initialized.
    /// </summary>
    public abstract class Feature
    {
        public abstract string Name { get; }
        public abstract MaterialIconKind Icon { get; }

        /// <summary>
        /// The priority by which feature will be initialized and ordered in lists
        /// </summary>
        public virtual float Priority => 0;

        /// <summary>
        /// Method that is called after MainWindow instance created
        /// </summary>
        public virtual void Init(App app) { }
    }
}
