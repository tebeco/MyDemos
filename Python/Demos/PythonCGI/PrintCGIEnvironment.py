#!/usr/bin/python

import os

print "Content-type: text/html\r\n\r\n";
print "Environment<\br>";
for param in os.environ.keys():
  print "<b>%20s</b>: %s<\br>" % (param, os.environ[param])