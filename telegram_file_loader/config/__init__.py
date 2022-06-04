# flake8: noqa
import ast
import logging
import sys
from os import getenv

from .base import *

logging.basicConfig(
    stream=sys.stdout,
    level=logging.INFO,
    format='[%(asctime)s] {%(name)s} %(levelname)s - %(message)s'
)
log = logging.getLogger(__name__)

try:
    from .local import *
except ImportError:
    log.info('Not found local.py')

# Override config variables from environment
for var in list(locals()):
    value = getenv(var)
    if value is None:
        continue
    try:
        log.info(f'Found new value for {var=}')
        locals()[var] = ast.literal_eval(value)
    except:  # noqa
        locals()[var] = value
