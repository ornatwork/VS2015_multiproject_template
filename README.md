# VS2015_multiproject_template
VS2015 multi project Template, that does not have double solution folder once code is created using the template.  

This is achieved by having a fake Template in root, then the real Template in another directory.  On the first run the RootWizard / IWizard will adjust where the soltuion is created using the real Template.

The solution will look something like this on the hard drive after it has been created

- Sample  
  sample.sln  
  global.json  
  ...  
  - test  
    - SampleTest.test  
      Testbase.cs  
      ...  
  - src  
     - Sample  
     Startup.cs  
     ...  
     
    
    