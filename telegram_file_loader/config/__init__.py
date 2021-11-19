# flake8: noqa
import ast

from .base import *
from os import getenv

try:
    from .local import *
except ImportError:
    print('Not found local.py')

# Override config variables from environment
for var in list(locals()):
    value = getenv(var)
    if value is None:
        continue
    try:
        locals()[var] = ast.literal_eval(value)
    except:  # noqa
        locals()[var] = value
