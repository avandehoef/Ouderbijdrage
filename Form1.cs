using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Ouderbijdrage
{
    /* Ouderbijdrage school
    Een basisschool berekent de ouderbijdrage als volgt:
    - Een basisbedrag van € 50,=. 
    - Daarnaast voor elk kind jonger dan 10 jaar € 25,= (voor maximaal 3 kinderen) 
    - Voor elk kind van 10 jaar en ouder € 37,= (voor maximaal 2 kinderen).
    - Voor éénoudergezinnen wordt op de berekende bijdrage (nádat de controle op het maximum heeft plaatsgevonden) een reductie toegepast van 25%.
    -- De maximale ouderbijdrage bedraagt € 150,=.
    
    De te ontwikkelen software moet aan de hand van de gezinsgegevens de verschuldigde ouderbijdrage bepalen. 
    De leeftijd van elk kind moet aan de hand van geboortedatum en een peildatum worden berekend.*/

    public partial class ouderBijdrage : Form
    {
        decimal totaal = 0.00m, subtotaal = 0.00m;
        decimal basisBedrag = 50.00m; // basisbedrag is € 50

        decimal bijdrageEenKindKleinerDanTien = 25.00m, bijdrageTweeKindKleinerDanTien = 50.00m, bijdrageDrieKindKleinerDanTien = 75.00m;
        decimal bijdrageEenKindGroterDanTien = 37.00m, bijdrageTweeKindGroterDanTien = 74.00m;

        decimal bijdrageKindKleinerTien = 0.00m, bijdrageKindGroterTien = 0.00m; // per kind van jonger dan 10 jaar op de peildatum wordt er een bedrag van € 25 gerekend, per kind 10 of groter > € 37 per kind
        
        decimal kortingEenOuder = 0.75m; // een éénoudergezin krijgt 25% korting (dus betaalt 75%) van het "berekende" tarief (niet over de basis bijdrage)
        DateTime peilDatum = DateTime.Now;

        int dagEen, dagTwee, dagDrie, dagVier, dagVijf;
        int maandEen, maandTwee, maandDrie, maandVier, maandVijf;
                
        public ouderBijdrage()
        {
            InitializeComponent();
        }

        private void ouderBijdrage_Load(object sender, EventArgs e)
        {
            foreach (var combobox in Controls.OfType<ComboBox>())
            {
                combobox.SelectedIndex = 0;
                combobox.Visible = false;
            }
            comboBoxKinderenKleinerDanTien.Visible = true;
            comboBoxKinderenTienOfGroter.Visible = true;

            foreach (var textbox in Controls.OfType<TextBox>())
            {
                textbox.Visible = false;
            }
            textPeilDatum.Visible = true;
            textPeilDatum.Text = "Peildatum: " + DateTime.Now.ToShortDateString();
            textAantalKinderenOnderTien.Visible = true;
            textAantalKinderenBovenTien.Visible = true;
            textAanhef.Visible = true;            
        }

        private void btnBereken_Click(object sender, EventArgs e)
        {
            if (comboBoxKinderenKleinerDanTien.Text == "0" && comboBoxKinderenTienOfGroter.Text == "0")
            {
                textFoutMelding.Visible = true;
                textFoutMelding.Text = "foutieve invoer: Geef het aantal kinderen en de juiste geboortedata op.";
                goto end;
            }

            bool eenKindJongerDanTien = comboBoxKinderenKleinerDanTien.Text == "1" && comboBoxKinderenTienOfGroter.Text == "0" && comboBoxKinderenTienOfGroter.Text == "0";
            bool tweeKindJongerDanTien = comboBoxKinderenKleinerDanTien.Text == "2" && comboBoxKinderenTienOfGroter.Text == "0" && comboBoxKinderenTienOfGroter.Text == "0";
            bool drieKindJongerDanTien = comboBoxKinderenKleinerDanTien.Text == "3 of meer" && comboBoxKinderenTienOfGroter.Text == "0" && comboBoxKinderenTienOfGroter.Text == "0";

            bool eenKindGroterDanTien = comboBoxKinderenTienOfGroter.Text == "1" && comboBoxKinderenKleinerDanTien.Text == "0" && comboBoxKinderenKleinerDanTien.Text == "0";
            bool tweeKindGroterDanTien = comboBoxKinderenTienOfGroter.Text == "2 of meer" && comboBoxKinderenKleinerDanTien.Text == "0" && comboBoxKinderenKleinerDanTien.Text == "0";

            bool eenKleinerEenGroter = comboBoxKinderenKleinerDanTien.Text == "1" && comboBoxKinderenTienOfGroter.Text == "1";
            bool eenKleinerTweeGroter = comboBoxKinderenKleinerDanTien.Text == "1" && comboBoxKinderenTienOfGroter.Text == "2 of meer"; 
            bool tweeKleinerEenGroter = comboBoxKinderenKleinerDanTien.Text == "2" && comboBoxKinderenTienOfGroter.Text == "1"; 
            bool tweeKleinerTweeGroter = comboBoxKinderenKleinerDanTien.Text == "2" && comboBoxKinderenTienOfGroter.Text == "2 of meer";
            bool drieKleinerEenGroter = comboBoxKinderenKleinerDanTien.Text == "3 of meer" && comboBoxKinderenTienOfGroter.Text == "1";
            bool drieKleinerTweeGroter = comboBoxKinderenKleinerDanTien.Text == "3 of meer" && comboBoxKinderenTienOfGroter.Text == "2 of meer";

            dagEen = Convert.ToInt16(comboBoxKleinerTienEersteKindDag.Text);
            maandEen = Convert.ToInt16((comboBoxKleinerTienEersteKindMaand.SelectedIndex) + 1);
            
            dagTwee = Convert.ToInt16(comboBoxKleinerTienTweedeKindDag.Text);
            maandTwee = Convert.ToInt16((comboBoxKleinerTienTweedeKindMaand.SelectedIndex) + 1);

            dagDrie = Convert.ToInt16(comboBoxKleinerTienDerdeKindDag.Text);
            maandDrie = Convert.ToInt16((comboBoxKleinerTienDerdeKindMaand.SelectedIndex) + 1);

            dagVier = Convert.ToInt16(comboBoxGroterTienEersteKindDag.Text);
            maandVier = Convert.ToInt16((comboBoxGroterTienEersteKindMaand.SelectedIndex) + 1);

            dagVijf = Convert.ToInt16(comboBoxGroterTienTweedeKindDag.Text);
            maandVijf = Convert.ToInt16((comboBoxGroterTienTweedeKindMaand.SelectedIndex) + 1);

            if (eenKindJongerDanTien)
            {
                if (comboBoxKleinerTienEersteKindJaar.Text != "2007")
                {
                    bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                }
                else
                {
                    if (maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: dit kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: dit kind is 10 of ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
            }
            else if (tweeKindJongerDanTien)
            {
                if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007")
                {
                    bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het eerste kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = textFoutMelding.Text = "foutieve invoer: het eerste kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text == "2007")
                {
                    if (maandTwee < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het tweede kind is 10 of ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandTwee > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                    }
                    else if (maandTwee == peilDatum.Month)
                    {
                        if (dagTwee <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het tweede kind is 10 of ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxKleinerTienTweedeKindJaar.Text == "2007")
                {
                    if (maandTwee < peilDatum.Month || maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandTwee > peilDatum.Month && maandEen > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                    }
                    else if (maandTwee == peilDatum.Month || maandEen == peilDatum.Month)
                    {
                        if (dagTwee <= peilDatum.Day || dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                        }
                    }
                }
            }
            else if (drieKindJongerDanTien)
            {
                if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007" && comboBoxKleinerTienDerdeKindJaar.Text != "2007")
                {
                    bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007" && comboBoxKleinerTienDerdeKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het eerste kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = textFoutMelding.Text = "foutieve invoer: het eerste kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text == "2007" && comboBoxKleinerTienDerdeKindJaar.Text != "2007")
                {
                    if (maandTwee < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het tweede kind is 10 of ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandTwee > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandTwee == peilDatum.Month)
                    {
                        if (dagTwee <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het tweede kind is 10 of ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxKleinerTienTweedeKindJaar.Text == "2007" && comboBoxKleinerTienDerdeKindJaar.Text == "2007")
                {
                    if (maandTwee < peilDatum.Month || maandEen < peilDatum.Month || maandDrie < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandTwee > peilDatum.Month && maandEen > peilDatum.Month && maandDrie > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandTwee == peilDatum.Month || maandEen == peilDatum.Month || maandDrie == peilDatum.Month)
                    {
                        if (dagTwee <= peilDatum.Day || dagEen <= peilDatum.Day || dagDrie <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007" && comboBoxKleinerTienDerdeKindJaar.Text == "2007")
                {
                    if (maandDrie < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het derde kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandDrie > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandDrie == peilDatum.Month)
                    {
                        if (dagDrie <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het derde kind is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxKleinerTienTweedeKindJaar.Text == "2007" && comboBoxKleinerTienDerdeKindJaar.Text != "2007")
                {
                    if (maandTwee < peilDatum.Month || maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandTwee > peilDatum.Month && maandEen > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandTwee == peilDatum.Month || maandEen == peilDatum.Month)
                    {
                        if (dagTwee <= peilDatum.Day || dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007" && comboBoxKleinerTienDerdeKindJaar.Text == "2007")
                {
                    if (maandDrie < peilDatum.Month || maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandDrie > peilDatum.Month && maandEen > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandDrie == peilDatum.Month || maandEen == peilDatum.Month)
                    {
                        if (dagDrie <= peilDatum.Day || dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text == "2007" && comboBoxKleinerTienDerdeKindJaar.Text == "2007")
                {
                    if (maandTwee < peilDatum.Month || maandDrie < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandTwee > peilDatum.Month && maandDrie > peilDatum.Month)
                    {
                        bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                    }
                    else if (maandTwee == peilDatum.Month || maandDrie == peilDatum.Month)
                    {
                        if (dagTwee <= peilDatum.Day || dagDrie <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is ouder dan 10; voer deze in het veld voor kinderen van 10 en ouder in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindKleinerTien = bijdrageDrieKindKleinerDanTien;
                        }
                    }
                }
            }
            else if (eenKindGroterDanTien) //dagVier en maandVier en jaarVier gaat om het eerste kind van 10 en groter. dagVijf en maandVijf en jaarVijf gaat om het tweede kind van 10 en groter
            {
                if (comboBoxGroterTienEersteKindJaar.Text != "2007")
                {
                    bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                }
                else
                {
                    if (maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: dit kind is nog geen 10; voer deze in het veld voor kinderen van 10 en jonger in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                    }
                    else if (maandVier == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: dit kind is nog geen 10; voer deze in het veld voor kinderen van 10 en jonger in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                        }
                    }
                }
            }
            else if (tweeKindGroterDanTien)
            {
                if (comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                }
                else if (comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het eerste kind is nog geen 10; voer deze in het veld voor kinderen van 10 en jonger in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                    }
                    else if (maandVier == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het eerste kind is nog geen 10; voer deze in het veld voor kinderen van 10 en jonger in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        }
                    }
                }
                else if (comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het tweede kind is nog geen 10; voer deze in het veld voor kinderen van 10 en jonger in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                    }
                    else if (maandVijf == peilDatum.Month)
                    {
                        if (dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het tweede kind is nog geen 10; voer deze in het veld voor kinderen van 10 en jonger in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        }
                    }
                }
                else if (comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandVier > peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is nog geen 10; voer deze in het veld voor kinderen jonger dan 10 in";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                    }
                    else if (maandVijf == peilDatum.Month || maandVier == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is nog geen 10; voer deze in het veld voor kinderen jonger dan 10 in";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        }
                    }
                }
            }
            else if (eenKleinerEenGroter)
            {
                if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007")
                {
                    bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                    bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007")
                {
                    if (maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: beide kinderen zijn jonger dan 10 jaar; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVier == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: beide kinderen zijn jonger dan 10 jaar; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: beide kinderen zijn ouder dan 10 jaar; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: beide kinderen zijn ouder dan 10 jaar; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007")
                {
                    if (maandEen < peilDatum.Month || maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van beide kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVier == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van beide kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
            }
            else if (eenKleinerTweeGroter)
            {
                if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                    bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het eerste kind wat is ingevuld bij 10 of ouder, is jonger dan 10 jaar; voer eea correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVier == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het eerste kind wat is ingevuld bij 10 of ouder, is jonger dan 10 jaar; voer eea correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het kind wat is ingevuld bij jonger dan 10, is ouder dan 10 jaar; voer eea correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het kind wat is ingevuld bij jonger dan 10, is ouder dan 10 jaar; voer eea correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandEen < peilDatum.Month || maandVier > peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVier < peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVier == peilDatum.Month || maandVijf == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVier >= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month || maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVier == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandEen < peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVijf == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVijf == peilDatum.Month)
                    {
                        if (dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandVier > peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVier == peilDatum.Month || maandVijf == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
            }
            else if (tweeKleinerEenGroter)
            {
                if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxKleinerTienTweedeKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007")
                {
                    bijdrageKindGroterTien = bijdrageEenKindGroterDanTien;
                    bijdrageKindKleinerTien = bijdrageTweeKindKleinerDanTien;
                } // hier gebleven hier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier geblevenhier gebleven
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het eerste kind wat is ingevuld bij 10 of ouder, is jonger dan 10 jaar; voer eea correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVier == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het eerste kind wat is ingevuld bij 10 of ouder, is jonger dan 10 jaar; voer eea correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: het kind wat is ingevuld bij jonger dan 10, is ouder dan 10 jaar; voer eea correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: het kind wat is ingevuld bij jonger dan 10, is ouder dan 10 jaar; voer eea correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandEen < peilDatum.Month || maandVier > peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVier < peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVier == peilDatum.Month || maandVijf == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVier >= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text != "2007")
                {
                    if (maandEen < peilDatum.Month || maandVier > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVier < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVier == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVier >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text == "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandEen < peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandEen > peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandEen == peilDatum.Month || maandVijf == peilDatum.Month)
                    {
                        if (dagEen <= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text != "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVijf == peilDatum.Month)
                    {
                        if (dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
                else if (comboBoxKleinerTienEersteKindJaar.Text != "2007" && comboBoxGroterTienEersteKindJaar.Text == "2007" && comboBoxGroterTienTweedeKindJaar.Text == "2007")
                {
                    if (maandVier > peilDatum.Month || maandVijf > peilDatum.Month)
                    {
                        textFoutMelding.Visible = true;
                        textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                        textResultaat.Text = "";
                        textTotaleBijdrageIs.Visible = false;
                        goto end;
                    }
                    else if (maandVier < peilDatum.Month && maandVijf < peilDatum.Month)
                    {
                        bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                        bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                    }
                    else if (maandVier == peilDatum.Month || maandVijf == peilDatum.Month)
                    {
                        if (dagVier >= peilDatum.Day || dagVijf >= peilDatum.Day)
                        {
                            textFoutMelding.Visible = true;
                            textFoutMelding.Text = "foutieve invoer: één van de kinderen is in een foutief veld in gevuld; voer ze correct in AUB";
                            textResultaat.Text = "";
                            textTotaleBijdrageIs.Visible = false;
                            goto end;
                        }
                        else
                        {
                            bijdrageKindGroterTien = bijdrageTweeKindGroterDanTien;
                            bijdrageKindKleinerTien = bijdrageEenKindKleinerDanTien;
                        }
                    }
                }
            }


            subtotaal = bijdrageKindKleinerTien + bijdrageKindGroterTien;
            if (subtotaal >= 100)
            {
                subtotaal = 100;
            }

            if (checkBoxEenOuderGezin.Checked == true)
            {
                subtotaal = subtotaal * kortingEenOuder;
            }

            totaal = basisBedrag + subtotaal;
            
            textResultaat.Visible = true;
            textTotaleBijdrageIs.Visible = true;
            textFoutMelding.Text = "";
            textResultaat.Text = totaal.ToString("C");
            textTotaleBijdrageIs.Text = "De totale bijdrage is: ";
            end:;
        }

        private void comboBoxKinderenKleinerDanTien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxKinderenKleinerDanTien.Text == "1")
            {
                comboBoxKleinerTienEersteKindDag.Enabled = true;
                comboBoxKleinerTienEersteKindMaand.Enabled = true;
                comboBoxKleinerTienEersteKindJaar.Enabled = true;
                comboBoxKleinerTienTweedeKindDag.Enabled = false;
                comboBoxKleinerTienTweedeKindMaand.Enabled = false;
                comboBoxKleinerTienTweedeKindJaar.Enabled = false;
                comboBoxKleinerTienDerdeKindDag.Enabled = false;
                comboBoxKleinerTienDerdeKindMaand.Enabled = false;
                comboBoxKleinerTienDerdeKindJaar.Enabled = false;

                comboBoxKleinerTienEersteKindDag.Visible = true;
                comboBoxKleinerTienEersteKindMaand.Visible = true;
                comboBoxKleinerTienEersteKindJaar.Visible = true;
                comboBoxKleinerTienTweedeKindDag.Visible = false;
                comboBoxKleinerTienTweedeKindMaand.Visible = false;
                comboBoxKleinerTienTweedeKindJaar.Visible = false;
                comboBoxKleinerTienDerdeKindDag.Visible = false;
                comboBoxKleinerTienDerdeKindMaand.Visible = false;
                comboBoxKleinerTienDerdeKindJaar.Visible = false;
                textBoxGebEersteKindMinTien.Visible = true;
                textBoxGebTweedeKindMinTien.Visible = false;
                textBoxGebDerdeKindMinTien.Visible = false;

                comboBoxKleinerTienTweedeKindDag.SelectedIndex = 0;
                comboBoxKleinerTienTweedeKindMaand.SelectedIndex = 0;
                comboBoxKleinerTienTweedeKindJaar.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindDag.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindMaand.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindJaar.SelectedIndex = 0;
            }
            else if (comboBoxKinderenKleinerDanTien.Text == "2")
            {
                comboBoxKleinerTienEersteKindDag.Enabled = true;
                comboBoxKleinerTienEersteKindMaand.Enabled = true;
                comboBoxKleinerTienEersteKindJaar.Enabled = true;
                comboBoxKleinerTienTweedeKindDag.Enabled = true;
                comboBoxKleinerTienTweedeKindMaand.Enabled = true;
                comboBoxKleinerTienTweedeKindJaar.Enabled = true;
                comboBoxKleinerTienDerdeKindDag.Enabled = false;
                comboBoxKleinerTienDerdeKindMaand.Enabled = false;
                comboBoxKleinerTienDerdeKindJaar.Enabled = false;

                comboBoxKleinerTienEersteKindDag.Visible = true;
                comboBoxKleinerTienEersteKindMaand.Visible = true;
                comboBoxKleinerTienEersteKindJaar.Visible = true;
                comboBoxKleinerTienTweedeKindDag.Visible = true;
                comboBoxKleinerTienTweedeKindMaand.Visible = true;
                comboBoxKleinerTienTweedeKindJaar.Visible = true;
                comboBoxKleinerTienDerdeKindDag.Visible = false;
                comboBoxKleinerTienDerdeKindMaand.Visible = false;
                comboBoxKleinerTienDerdeKindJaar.Visible = false;
                textBoxGebEersteKindMinTien.Visible = true;
                textBoxGebTweedeKindMinTien.Visible = true;
                textBoxGebDerdeKindMinTien.Visible = false;

                comboBoxKleinerTienDerdeKindDag.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindMaand.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindJaar.SelectedIndex = 0;
            }
            else if (comboBoxKinderenKleinerDanTien.Text == "3 of meer")
            {
                comboBoxKleinerTienEersteKindDag.Enabled = true;
                comboBoxKleinerTienEersteKindMaand.Enabled = true;
                comboBoxKleinerTienEersteKindJaar.Enabled = true;
                comboBoxKleinerTienTweedeKindDag.Enabled = true;
                comboBoxKleinerTienTweedeKindMaand.Enabled = true;
                comboBoxKleinerTienTweedeKindJaar.Enabled = true;
                comboBoxKleinerTienDerdeKindDag.Enabled = true;
                comboBoxKleinerTienDerdeKindMaand.Enabled = true;
                comboBoxKleinerTienDerdeKindJaar.Enabled = true;

                comboBoxKleinerTienEersteKindDag.Visible = true;
                comboBoxKleinerTienEersteKindMaand.Visible = true;
                comboBoxKleinerTienEersteKindJaar.Visible = true;
                comboBoxKleinerTienTweedeKindDag.Visible = true;
                comboBoxKleinerTienTweedeKindMaand.Visible = true;
                comboBoxKleinerTienTweedeKindJaar.Visible = true;
                comboBoxKleinerTienDerdeKindDag.Visible = true;
                comboBoxKleinerTienDerdeKindMaand.Visible = true;
                comboBoxKleinerTienDerdeKindJaar.Visible = true;
                textBoxGebEersteKindMinTien.Visible = true;
                textBoxGebTweedeKindMinTien.Visible = true;
                textBoxGebDerdeKindMinTien.Visible = true;
            }
            else if (comboBoxKinderenKleinerDanTien.Text == "0")
            {
                comboBoxKleinerTienEersteKindDag.Enabled = false;
                comboBoxKleinerTienEersteKindMaand.Enabled = false;
                comboBoxKleinerTienEersteKindJaar.Enabled = false;
                comboBoxKleinerTienTweedeKindDag.Enabled = false;
                comboBoxKleinerTienTweedeKindMaand.Enabled = false;
                comboBoxKleinerTienTweedeKindJaar.Enabled = false;
                comboBoxKleinerTienDerdeKindDag.Enabled = false;
                comboBoxKleinerTienDerdeKindMaand.Enabled = false;
                comboBoxKleinerTienDerdeKindJaar.Enabled = false;

                comboBoxKleinerTienEersteKindDag.Visible = false;
                comboBoxKleinerTienEersteKindMaand.Visible = false;
                comboBoxKleinerTienEersteKindJaar.Visible = false;
                comboBoxKleinerTienTweedeKindDag.Visible = false;
                comboBoxKleinerTienTweedeKindMaand.Visible = false;
                comboBoxKleinerTienTweedeKindJaar.Visible = false;
                comboBoxKleinerTienDerdeKindDag.Visible = false;
                comboBoxKleinerTienDerdeKindMaand.Visible = false;
                comboBoxKleinerTienDerdeKindJaar.Visible = false;
                textBoxGebEersteKindMinTien.Visible = false;
                textBoxGebTweedeKindMinTien.Visible = false;
                textBoxGebDerdeKindMinTien.Visible = false;

                comboBoxKleinerTienEersteKindDag.SelectedIndex = 0;
                comboBoxKleinerTienEersteKindMaand.SelectedIndex = 0;
                comboBoxKleinerTienEersteKindJaar.SelectedIndex = 0;
                comboBoxKleinerTienTweedeKindDag.SelectedIndex = 0;
                comboBoxKleinerTienTweedeKindMaand.SelectedIndex = 0;
                comboBoxKleinerTienTweedeKindJaar.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindDag.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindMaand.SelectedIndex = 0;
                comboBoxKleinerTienDerdeKindJaar.SelectedIndex = 0;
            }
        }

        private void comboBoxKinderenTienOfGroter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxKinderenTienOfGroter.Text == "1")
            {
                comboBoxGroterTienEersteKindDag.Enabled = true;
                comboBoxGroterTienEersteKindDag.Visible = true;
                comboBoxGroterTienEersteKindMaand.Enabled = true;
                comboBoxGroterTienEersteKindMaand.Visible = true;
                comboBoxGroterTienEersteKindJaar.Enabled = true;
                comboBoxGroterTienEersteKindJaar.Visible = true;

                comboBoxGroterTienTweedeKindDag.Enabled = false;
                comboBoxGroterTienTweedeKindMaand.Enabled = false;
                comboBoxGroterTienTweedeKindJaar.Enabled = false;
                comboBoxGroterTienTweedeKindDag.Visible = false;
                comboBoxGroterTienTweedeKindMaand.Visible = false;
                comboBoxGroterTienTweedeKindJaar.Visible = false;

                textBoxGebEersteKindPlusTien.Visible = true;
                textBoxGebTweedeKindPlusTien.Visible = false;

                comboBoxGroterTienTweedeKindDag.SelectedIndex = 0;
                comboBoxGroterTienTweedeKindMaand.SelectedIndex = 0;
                comboBoxGroterTienTweedeKindJaar.SelectedIndex = 0;
            }
            else if (comboBoxKinderenTienOfGroter.Text == "2 of meer")
            {
                comboBoxGroterTienEersteKindDag.Enabled = true;
                comboBoxGroterTienEersteKindDag.Visible = true;
                comboBoxGroterTienEersteKindMaand.Enabled = true;
                comboBoxGroterTienEersteKindMaand.Visible = true;
                comboBoxGroterTienEersteKindJaar.Enabled = true;
                comboBoxGroterTienEersteKindJaar.Visible = true;

                comboBoxGroterTienTweedeKindDag.Enabled = true;
                comboBoxGroterTienTweedeKindDag.Visible = true;
                comboBoxGroterTienTweedeKindMaand.Enabled = true;
                comboBoxGroterTienTweedeKindMaand.Visible = true;
                comboBoxGroterTienTweedeKindJaar.Enabled = true;
                comboBoxGroterTienTweedeKindJaar.Visible = true;

                textBoxGebEersteKindPlusTien.Visible = true;
                textBoxGebTweedeKindPlusTien.Visible = true;
            }
            else if (comboBoxKinderenTienOfGroter.Text == "0")
            {
                comboBoxGroterTienEersteKindDag.Enabled = false;
                comboBoxGroterTienEersteKindDag.Visible = false;
                comboBoxGroterTienEersteKindMaand.Enabled = false;
                comboBoxGroterTienEersteKindMaand.Visible = false;
                comboBoxGroterTienEersteKindJaar.Enabled = false;
                comboBoxGroterTienEersteKindJaar.Visible = false;

                comboBoxGroterTienTweedeKindDag.Enabled = false;
                comboBoxGroterTienTweedeKindDag.Visible = false;
                comboBoxGroterTienTweedeKindMaand.Enabled = false;
                comboBoxGroterTienTweedeKindMaand.Visible = false;
                comboBoxGroterTienTweedeKindJaar.Enabled = false;
                comboBoxGroterTienTweedeKindJaar.Visible = false;

                textBoxGebEersteKindPlusTien.Visible = false;
                textBoxGebTweedeKindPlusTien.Visible = false;

                comboBoxGroterTienEersteKindDag.SelectedIndex = 0;
                comboBoxGroterTienEersteKindMaand.SelectedIndex = 0;
                comboBoxGroterTienEersteKindJaar.SelectedIndex = 0;
                comboBoxGroterTienTweedeKindDag.SelectedIndex = 0;
                comboBoxGroterTienTweedeKindMaand.SelectedIndex = 0;
                comboBoxGroterTienTweedeKindJaar.SelectedIndex = 0;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (var combobox in Controls.OfType<ComboBox>())
            {
                combobox.SelectedIndex = 0;
                combobox.Enabled = false;
            }
            comboBoxKinderenKleinerDanTien.Enabled = true;
            comboBoxKinderenTienOfGroter.Enabled = true;
            textResultaat.Text = "";
            textTotaleBijdrageIs.Text = "";
            checkBoxEenOuderGezin.Checked = false;
            textFoutMelding.Text = "";
            textFoutMelding.Visible = false;
        }
    }
}
