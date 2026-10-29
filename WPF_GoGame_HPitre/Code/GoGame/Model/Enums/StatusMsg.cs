namespace GoGame.Model.Enums
{
    public static class StatusMsg
    {
        //https://chatgpt.com/share/69fff26f-a400-83ea-8d35-d41fa5218f0a

        public const string ErrGameFinished =
            "La partie est déjà dans les livres !";

        public const string ErrCellOccupied =
            "Oups, cette case est déjà prise !";

        public const string ErrSuicideMove =
            "Attention, ce coup ne survivra pas !";

        public const string ErrInvalidMove =
            "Hmm... ce coup ne marche pas ici !";

        public const string ErrUnknown =
            "Oups, quelque chose a dérapé !";

        public const string ErrKoRule =
            "Bien tenté — change un peu la situation ! (Règle du Ko)";

        public const string ErrOutOfBounds =
            "Doucement, c'est en dehors du plateau !";

        public const string ErrGameNotStarted =
            "La partie attend encore le premier coup !";

        #region Unimplemented

        public const string ErrNotYourTurn =
            "Hop là, ce n'est pas encore ton tour !";

        public const string ErrCannotPass =
            "Pas possible de passer tout de suite !";

        public const string ErrAlreadyPassed =
            "Tu as déjà passé ton tour !";

        public const string ErrUndoUnavailable =
            "Pas de retour arrière cette fois !";

        public const string ErrDisconnected =
            "Connexion perdue... on reconnecte ça.";

        public const string ErrSaveFailed =
            "Impossible de sauvegarder la partie !";

        public const string ErrLoadFailed =
            "Impossible de charger la partie !";

        #endregion
    }
}