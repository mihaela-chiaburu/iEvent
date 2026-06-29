using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Infrastructure.ExternalServices
{
    public static class CloudinaryUrlHelper
    {
        public static string Banner(string url)
        {
            return url.Replace(
                "/upload/",
                "/upload/f_auto,q_auto,w_1600,h_600,c_fill/");
        }

        public static string Gallery(string url)
        {
            return url.Replace(
                "/upload/",
                "/upload/f_auto,q_auto,w_800,h_600,c_fill/");
        }

        public static string Thumbnail(string url)
        {
            return url.Replace(
                "/upload/",
                "/upload/f_auto,q_auto,w_200,h_200,c_fill/");
        }

        public static string Original(string url)
        {
            return url;
        }
    }
}
