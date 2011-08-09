using System.IO;
using System.Linq;
using EyePatch.Core.Util;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class MediaFolderNode : Node
    {
        public MediaFolderNode()
        {
            CssClass = "folder";
            Type = "mediaFolder";
        }

        public MediaFolderNode(DirectoryInfo dir)
            : this()
        {
            Id = PathHelper.PhysicalToUrl(dir.FullName);
            Name = dir.Name;

            dir.GetDirectories().ToList().ForEach(AddFolder);
        }

        private void AddFolder(DirectoryInfo dir)
        {
            Children.Add(new MediaFolderNode(dir));
        }
    }
}