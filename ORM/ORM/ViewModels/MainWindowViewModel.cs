using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ORM.Models;

namespace ORM.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public string Greeting => "Welcome to Avalonia!";

    public MainWindowViewModel() {
        using (var context = new MonContexte()) {

            //var blogs = context.Database.SqlQuery<List<Blog>>($"SELECT * FROM get_blogs()").ToList();


            /*//var blog = context.Blogs.Where(b => b.Id == 1).First();
            var blog2 = context.Blogs.Include(b => b.Posts).SingleOrDefault(b => b.Id == 1);
            //var blogs = context.Blogs.ToList();
            blog2.Url = "goooogle";
            //context.Blogs.Remove(blog2);
            context.SaveChanges();*/

            /*var blog = new Blog { Url = "google", Posts = new List<Post>()};
            context.Blogs.Add(blog);

            var post1 = new Post {
                Title = "a",
                Content = "AAAAAAAAAAAAAAAAAAAA"
            };
            blog.Posts.Add(post1);
            post1.Blog = blog;

            context.SaveChanges();*/
        }
    }
}