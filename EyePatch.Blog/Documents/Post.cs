using System;
using System.Collections.Generic;
using EyePatch.Core.Audit;
using EyePatch.Core.Mvc.Sitemap;

namespace EyePatch.Blog.Documents
{
    public class Post : ISiteMapItem, IAuditCreatedDate, IAuditModifiedDate
    {
        private const string defaultBody =
            @"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus adipiscing vestibulum lectus, vitae volutpat odio facilisis non. Suspendisse leo sem, tincidunt vitae porttitor vel, pulvinar viverra diam. Vivamus vel vestibulum neque. Nunc nec magna sed mi dictum hendrerit in vitae lacus. Nulla urna quam, adipiscing id suscipit nec, condimentum sed elit. Integer ligula tellus, iaculis eget consequat accumsan, tristique eu lacus. Praesent a augue elit, et porttitor ligula. Aenean a fermentum mi. Nulla sagittis dictum venenatis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam convallis dapibus neque vestibulum blandit. Ut ornare fermentum mauris ut accumsan. Praesent rhoncus justo sed metus ullamcorper malesuada faucibus vel lorem. Fusce feugiat fermentum dolor nec hendrerit. Etiam ullamcorper metus ut magna mollis egestas.</p>
                                            <p>Vivamus venenatis commodo tortor eget euismod. Nam viverra rutrum lorem vitae aliquet. Vestibulum nec libero vestibulum leo laoreet dapibus. Maecenas fermentum facilisis quam, in cursus felis sagittis ut. Phasellus convallis, metus at pellentesque iaculis, tellus risus mollis odio, eu vulputate est libero sed nulla. Nunc lectus urna, pretium id hendrerit tincidunt, vehicula a massa. Phasellus vel tristique justo. Praesent pulvinar eros a nisl volutpat tempus lacinia sed metus. In hac habitasse platea dictumst. Nunc a nisl quam, sit amet ultrices mi. Duis eu porta risus. Aliquam sit amet volutpat turpis. In facilisis sapien eget magna viverra volutpat.</p>
                                            <p>Maecenas porta consectetur augue, quis aliquet ligula ornare vel. Proin dignissim euismod commodo. Mauris tincidunt elit a turpis porttitor vestibulum cursus erat pellentesque. Praesent congue convallis eros non pretium. Aenean quis ligula dolor. Duis posuere placerat felis, vitae feugiat erat commodo quis. Aenean luctus dui eget dolor condimentum sodales. Nulla metus diam, tristique a tincidunt eu, malesuada sit amet odio. Praesent fermentum, lectus quis faucibus accumsan, lorem metus porttitor odio, ut consectetur mauris mi et nisl. Aliquam sed augue sit amet est dapibus scelerisque gravida a ipsum. Mauris ac dolor tortor. Fusce at leo lobortis justo volutpat egestas sit amet condimentum dui. Mauris suscipit fermentum turpis, nec condimentum dui sagittis sed. Etiam lobortis mi eget tortor congue eget tempus diam aliquet. Etiam non lacinia mauris. Vestibulum tempor nisi eu purus tincidunt at pulvinar nulla porta. Aliquam erat volutpat.</p>
                                            <p>Proin tortor urna, viverra at accumsan in, volutpat et turpis. Praesent nulla arcu, vehicula ultrices varius in, porttitor sit amet dui. Aliquam vulputate blandit metus, in aliquam enim malesuada quis. Suspendisse pretium nibh suscipit neque facilisis a commodo risus dignissim. Suspendisse potenti. Proin at ultricies est. Ut interdum nulla tempus nibh mattis ultricies. Curabitur egestas urna non nulla mollis eu ornare lectus posuere. Nulla rutrum, dolor et euismod porttitor, nulla tortor porta urna, et posuere tellus felis sed arcu. Donec suscipit aliquet tortor, ac iaculis magna auctor in. Nunc cursus ullamcorper quam, nec ultricies velit tincidunt in. Donec eget turpis sem, eget aliquet augue. Nullam mauris urna, pellentesque a posuere eget, imperdiet sed erat. Maecenas viverra nisi a enim dapibus aliquam. Proin dignissim enim eu purus lacinia ut feugiat arcu sollicitudin. Pellentesque eget lectus mi. Nullam a massa magna, sed tristique augue. Ut cursus mattis sem hendrerit volutpat. Nam porta justo sed odio luctus non fermentum massa posuere.</p>
                                            <p>Ut ut elit eu augue pharetra auctor ut sit amet odio. Aliquam hendrerit turpis erat, id faucibus mauris. Mauris non rutrum mi. Donec eu erat orci. Cras in dui metus, id dignissim leo. In sollicitudin metus eu diam mattis commodo. Phasellus semper hendrerit fermentum. Nullam rutrum odio vitae nisl convallis in varius odio congue. Pellentesque elementum neque eget magna iaculis id adipiscing risus dapibus. Ut dignissim rhoncus leo eget vehicula. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Sed libero augue, venenatis non mattis sed, fermentum at libero. Morbi id odio et nulla aliquet rhoncus at nec ipsum. Mauris volutpat ornare risus, ut dictum lectus tristique quis. Nam in risus sed nunc laoreet aliquet. Fusce quis erat et magna laoreet mollis.</p>";

        public Post()
        {
            Priority = 0.8;
            ChangeFrequency = Core.Mvc.Sitemap.ChangeFrequency.Monthly;
            Body = defaultBody;
            Tags = new List<Tag>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime? Published { get; set; }
        public List<Tag> Tags { get; protected set; }

        // audit

        #region IAuditCreatedDate Members

        public DateTime Created { get; set; }

        #endregion

        #region ISiteMapItem Members

        public string Url { get; set; }
        public DateTime? LastModified { get; set; }

        // collections

        // sitemap
        public ChangeFrequency? ChangeFrequency { get; set; }
        public double? Priority { get; set; }

        #endregion
    }
}