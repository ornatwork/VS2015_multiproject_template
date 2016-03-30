# VS2015_multiproject_template
VS2015 multi project template, that does not have double solution folder once code is created using the template.

This is achieved by having a fake Template in root, then the real Template in another directory.  On the first run the RootWizard will adjust where the soltuion is created using the real Template.

