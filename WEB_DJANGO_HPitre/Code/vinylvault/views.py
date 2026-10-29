from django.shortcuts import render, get_object_or_404, redirect
from django.contrib import messages
from django.contrib.auth.decorators import login_required
from .models import Album, AlbumUtilisateur, Artiste, Song
from .forms import ArtisteForm, AlbumForm, AlbumUtilisateurForm, SongForm
from django.core.paginator import Paginator
from django.utils.translation import gettext as _

#j'utilise cette façon de permission car la démo utilise des classes et pas des views
from django.contrib.auth.decorators import permission_required 

def accueil(request):
    return render(request, 'vinylvault/accueil.html')

def a_propos(request):
    return render(request, 'vinylvault/a_propos.html')

def liste_albums(request):
    albums = Album.objects.all()

    #nb items per page
    paginator = Paginator(albums, 9)
    page_number = request.GET.get('page') or 1

    #ajouter le détail de la page en contexte (pour comprendre le param en ?get )
    favoris_ids = []
    if request.user.is_authenticated:
        favoris_ids = AlbumUtilisateur.objects.filter(user=request.user, isFavori=True).values_list('album_id', flat=True)

    context = {
        'range_pages': range(1, paginator.num_pages + 1),
        'page_albums': paginator.get_page(page_number),
        'favoris_ids': list(favoris_ids),
    }

    return render(request, 'vinylvault/album/liste_albums.html', context)
    

def detail_album(request, pk):
    album = get_object_or_404(Album, pk=pk)

    # infos perso du user connecté sur cet album (None si pas connecté ou jamais modifié)
    infos = None
    form_infos = None
    if request.user.is_authenticated:
        infos = AlbumUtilisateur.objects.filter(user=request.user, album=album).first()
        form_infos = AlbumUtilisateurForm(instance=infos)  # form direct dans la section "Ma collection"

    return render(request, 'vinylvault/album/detail_album.html', {'album': album, 'infos': infos, 'form_infos': form_infos})


@login_required
def modifier_infos_album(request, pk):
    # statut / note / coup de coeur du user connecté seulement, l'album lui-meme change pas
    album = get_object_or_404(Album, pk=pk)
    if request.method != 'POST':
        return redirect('vinylvault:detail_album', pk=album.pk)

    infos = AlbumUtilisateur.objects.filter(user=request.user, album=album).first()
    form = AlbumUtilisateurForm(request.POST, instance=infos)
    if form.is_valid():
        infos = form.save(commit=False)
        infos.user = request.user
        infos.album = album
        infos.save()
        messages.success(request, _("Ta collection a été mise à jour."))
        return redirect('vinylvault:detail_album', pk=album.pk)

    messages.error(request, _("Le formulaire contient des erreurs, vérifiez les champs."))
    return render(request, 'vinylvault/album/detail_album.html', {'album': album, 'infos': infos, 'form_infos': form})

def liste_artistes(request):
    artistes = Artiste.objects.all()
    return render(request, 'vinylvault/artiste/liste_artistes.html', {'artistes': artistes})

#=========================== Artiste
@permission_required('vinylvault.add_artiste',raise_exception=True)
def ajouter_artiste(request):
    if request.method == 'POST':
        form = ArtisteForm(request.POST)
        if form.is_valid():
            form.save()
            messages.success(request, _("L'artiste a été ajouté avec succès."))
            return redirect('vinylvault:liste_artistes')
        else:
            messages.error(request, _("Le formulaire contient des erreurs, vérifiez les champs."))
    else:
        form = ArtisteForm()
    return render(request, 'vinylvault/artiste/artiste_form.html', {'form': form, 'titre': _('Ajouter un artiste')})

@permission_required('vinylvault.change_artiste',raise_exception=True)
def modifier_artiste(request, pk):
    artiste = get_object_or_404(Artiste, pk=pk)
    if request.method != 'POST':
        form = ArtisteForm(instance=artiste)
        return render(request, 'vinylvault/artiste/artiste_form.html', {'form': form, 'titre': _("Modifier l'artiste")})

    form = ArtisteForm(request.POST, instance=artiste)
    if not form.is_valid():
        messages.error(request, _("Le formulaire contient des erreurs, vérifiez les champs."))
        return render(request, 'vinylvault/artiste/artiste_form.html', {'form': form, 'titre': _("Modifier l'artiste")})

    form.save()
    messages.success(request, _("L'artiste a été modifié avec succès."))
    return redirect('vinylvault:liste_artistes')

@permission_required('vinylvault.delete_artiste',raise_exception=True)
def supprimer_artiste(request, pk):
    artiste = get_object_or_404(Artiste, pk=pk)
    if request.method == 'POST':
        nom = artiste.nom
        artiste.delete()
        messages.success(request, _("L'artiste « %(nom)s » a été supprimé.") % {'nom': nom})
        return redirect('vinylvault:liste_artistes')
    return render(request, 'vinylvault/artiste/artiste_confirm_delete.html', {'artiste': artiste})


#=========================== Album
@permission_required('vinylvault.add_album',raise_exception=True)
def ajouter_album(request):
    if request.method == 'POST':
        form = AlbumForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            messages.success(request, _("L'album a été ajouté avec succès."))
            return redirect('vinylvault:liste_albums')
        else:
            messages.error(request, _("Le formulaire contient des erreurs, vérifiez les champs."))
    else:
        form = AlbumForm()
    return render(request, 'vinylvault/album/album_form.html', {'form': form, 'titre': _('Ajouter un album')})

@permission_required('vinylvault.change_album',raise_exception=True)
def modifier_album(request, pk):
    album = get_object_or_404(Album, pk=pk)
    if request.method == 'POST':
        form = AlbumForm(request.POST, request.FILES, instance=album)
        if form.is_valid():
            form.save()
            messages.success(request, _("L'album a été modifié avec succès."))
            return redirect('vinylvault:detail_album', pk=album.pk)
        else:
            messages.error(request, _("Le formulaire contient des erreurs, vérifiez les champs."))
    else:
        form = AlbumForm(instance=album)
    return render(request, 'vinylvault/album/album_form.html', {'form': form, 'titre': _("Modifier l'album")})

@permission_required('vinylvault.delete_album',raise_exception=True)
def supprimer_album(request, pk):
    album = get_object_or_404(Album, pk=pk)
    if request.method == 'POST':
        titre = album.titre
        album.delete()
        messages.success(request, _("L'album « %(titre)s » a été supprimé.") % {'titre': titre})
        return redirect('vinylvault:liste_albums')
    return render(request, 'vinylvault/album/album_confirm_delete.html', {'album': album})

#=========================== chanson
@permission_required('vinylvault.add_song',raise_exception=True)
def ajouter_chanson(request, album_pk):
    # album_pk vient direct de l'URL, pas besoin de le faire choisir dans le formulaire
    album = get_object_or_404(Album, pk=album_pk)
    if request.method == 'POST':
        form = SongForm(request.POST, album=album)
        if form.is_valid():
            song = form.save(commit=False)
            song.album = album
            song.save()
            messages.success(request, _("La chanson a été ajoutée avec succès."))
            return redirect('vinylvault:detail_album', pk=album.pk)
        else:
            messages.error(request, _("Le formulaire contient des erreurs, vérifiez les champs."))
    else:
        form = SongForm(album=album)
    return render(request, 'vinylvault/song_form.html', {'form': form, 'album': album})

#========================== Permission
def not_allowed(request, exception=None):
    #called quand permission exception
    if request.user.is_authenticated:
        messages.error(request, _("Ton compte n'a pas la permission de faire cette action."))
    else:
        messages.error(request, _("Tu dois être connecté pour faire cette action."))

    #return à la page ou t'étais si dispo ou à home
    return redirect('vinylvault:liste_albums')