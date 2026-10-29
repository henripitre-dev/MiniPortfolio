"""Charge les données de démonstration et crée les comptes de test.

Usage : python manage.py seed_demo
"""
from django.contrib.auth import get_user_model
from django.contrib.auth.models import Group
from django.core.management import call_command
from django.core.management.base import BaseCommand

# (nom d'utilisateur, mot de passe, groupe, administrateur)
DEMO_USERS = [
    ("admin", "admin", None, True),
    ("gestionnaire", "demo1234", "Gestionnaires", False),
    ("collectionneur", "demo1234", "Collectionneurs", False),
]


class Command(BaseCommand):
    help = "Charge les données de démonstration et crée les comptes de test."

    def handle(self, *args, **options):
        call_command("loaddata", "initial_data")

        User = get_user_model()
        for username, password, group_name, is_admin in DEMO_USERS:
            user, _ = User.objects.get_or_create(username=username)
            user.set_password(password)
            user.is_staff = is_admin
            user.is_superuser = is_admin
            user.save()
            if group_name:
                user.groups.add(Group.objects.get(name=group_name))
            self.stdout.write(f"Compte prêt : {username} / {password}")
