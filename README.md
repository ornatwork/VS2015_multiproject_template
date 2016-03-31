# VS2015_multiproject_template
VS2015 multi project Template, that does not have double solution folder once code is created using the template.  

This is achieved by having a fake Template in root, then the real Template in another directory.  On the first run the RootWizard / IWizard will adjust where the solution is created using the real Template.  It will also use tmp_root to bring in root files and copy them in the correct root location.

The solution will look something like this below on the hard drive after it has been created

```
|--Sample  
  |--sample.sln  
  |--global.json  
  |--...  
  |--test    
     |--Sample.test  
        |--Testbase.cs  
        |--...  
  |--src
     |--Sample  
       |--Startup.cs  
       |--...  
```
     
    
    