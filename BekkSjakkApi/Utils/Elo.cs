using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BekkSjakkApi.Models;

namespace BekkSjakkApi.Utils
{
    public static class Elo
    {
        public const int KFaktor = 32;

        // Implementasjon følger https://metinmediamath.wordpress.com/2013/11/27/how-to-calculate-the-elo-rating-including-example/
        public static Tuple<int, int> FinnNyeEloRatinger(Bekker hvit, Bekker svart, PartiResultat resultat)
        {
            double ratingTransformertHvit = Math.Pow(10, (double)hvit.Elo / 400);
            double ratingTransformertSvart = Math.Pow(10, (double)svart.Elo/ 400);

            double forventningsverdiHvit = ratingTransformertHvit / (ratingTransformertHvit + ratingTransformertSvart);
            double forventningsverdiSvart = ratingTransformertSvart / (ratingTransformertHvit + ratingTransformertSvart);

            double poengHvit = resultat == PartiResultat.VinnerHvit ? 1 : 0;
            double poengSvart = resultat == PartiResultat.VinnerSvart ? 1 : 0;
            if (resultat == PartiResultat.Uavgjort)
            {
                poengHvit = 0.5;
                poengSvart = 0.5;
            }

            double nyRatingHvit = hvit.Elo + KFaktor * (poengHvit - forventningsverdiHvit);
            double nyRatingSvart = svart.Elo + KFaktor * (poengSvart - forventningsverdiSvart);
            return new Tuple<int, int>((int)nyRatingHvit, (int)nyRatingSvart);
        }
    }
}
