**EyePatch CMS** is an experiment CMS based on ASP.NET MVC & [Knockout](http://knockoutjs.com/). 

The goal of this project was to see if it was possible to administer a website without using the traditional UI and to do everything "in-place" via a rich javascript UI that is injected into a web page via the back-end.

The feature set of EyePatch is fairly limited and is designed with the bare minimum of features to be able to run my personal homepage and blog.

Hope this is interesting take in the Yet Another CMS series and you find something here. I will be working to expand the feature set as time goes on, but would welcome contributions.

Features :
----------

* No back-end UI aka Wordpress, a floating Knockout UI window as the main UI. 
* Uses the super fast Raven DB document store embedded, no install required.
* Compatible with .NET output caching.
* App can use a mixture of managed and unmanaged pages, the app is simply a ASP.NET web application with "extras"
* Plugin Framework allows extensions
* Basic blog functionality written as a plugin to the main UI. Has it own distinct UI which is also injected.
* Simple Razor view templates.
* 100% Ajax, no page reloads, all content is edited in-line.

License: MIT [http://www.opensource.org/licenses/mit-license.php](http://www.opensource.org/licenses/mit-license.php)
