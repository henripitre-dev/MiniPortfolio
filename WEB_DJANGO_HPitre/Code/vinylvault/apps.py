from django.apps import AppConfig

class VinylvaultConfig(AppConfig):
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'vinylvault'
    verbose_name = 'VinylVault'

    def ready(self):
        import vinylvault.signals