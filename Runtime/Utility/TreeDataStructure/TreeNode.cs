

namespace DyrdaDev.FirstPersonController
{
    public abstract class TreeNode
    {
        // just a test, not actually in use
        public TreeNode yesChild;
        public TreeNode noChild;

        public TreeNode(TreeNode yesChild, TreeNode noChild)
        {
            this.yesChild = yesChild;
            this.noChild = noChild;
        }
    }
}