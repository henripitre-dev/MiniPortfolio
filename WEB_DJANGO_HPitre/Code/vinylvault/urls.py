from django.urls import path
from . import views


app_name = 'vinylvault'  #  pour {% url 'vinylvault:x' %}  au lieu de hardcode

# "name" = call {% url %} pour ds les templates
urlpatterns = [
    path('', views.accueil, name='accueil'),
    path('a-propos/', views.a_propos, name='a_propos'),
    path('albums/', views.liste_albums, name='liste_albums'),
    path('albums/<int:pk>/', views.detail_album, name='detail_album'),
    path('albums/<int:pk>/mes-infos/', views.modifier_infos_album, name='modifier_infos_album'),

    path('artistes/', views.liste_artistes, name='liste_artistes'),
    path('artistes/ajouter/', views.ajouter_artiste, name='ajouter_artiste'),
    path('artistes/<int:pk>/modifier/', views.modifier_artiste, name='modifier_artiste'),
    path('artistes/<int:pk>/supprimer/', views.supprimer_artiste, name='supprimer_artiste'),

    path('albums/ajouter/', views.ajouter_album, name='ajouter_album'),
    path('albums/<int:pk>/modifier/', views.modifier_album, name='modifier_album'),
    path('albums/<int:pk>/supprimer/', views.supprimer_album, name='supprimer_album'),

    path('albums/<int:album_pk>/chansons/ajouter/', views.ajouter_chanson, name='ajouter_chanson'),
]

