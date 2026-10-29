from django.apps import AppConfig
from django.utils.translation import gettext_lazy as _


class MemberConfig(AppConfig):
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'member_app'
    verbose_name = _('Membres')
