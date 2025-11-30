# SnackToSixPack

## Beskrivning
SnackToSixPack är en C# konsolapplikation som huvudsakligen använder OpenAI för att generera ett träningsschema 
utifrån profilen som man fyllt i och beskrivningen man ger när man skapar en träningsplan. Programmet körs lokalt och kräver att användaren själv konfigurerar sina API-nycklar innan användning.

## Krav
- En giltig OpenAI API-nyckel
- Internetanslutning (för API-anrop)
- Sätt miljövariablerna
- Appen använder NuGet paket. Kör en NuGet installation innan appen körs för första gången.

## API-nyckel (måste göras innan programmet körs)
För att kunna använda OpenAI-funktionerna krävs att användaren sätter sin API-nyckel som en miljövariabel på datorn.

### Windows (powershell)
setx OPENAI_API_KEY "din-nyckel-här"
    
MAC/Linux
export OPENAI_API_KEY="din-nyckel-här"

#### Miljövariabeler för att skicka mejl
Windows (PowerShell):
setx MAIL_AUTH_UN "din-mailadress"
setx MAIL_AUTH_PW "ditt-app-password"

macOS/Linux (bash/zsh):
export MAIL_AUTH_UN="din-mailadress"
export MAIL_AUTH_PW="ditt-app-password"

- Det viktiga är att app-lösenord är inte synligt förrän du har aktiverat 2-stegsverifiering.
- Vi använder oss utav Gmail och skapar applösenord på vårat egna privata Google konto genom denna länk: https://myaccount.google.com/signinoptions/twosv?rapt=AEjHL4PQYWP-py1CQTY39dxFlxMTIgu8MftMwi-87TtgHR4voeAFtqCwokp9puiRToNH95OMgwPFH3TrcSCqnJcSHVSrn5vPznPiaNB8ZpAnNNXpqmd-Rt8
- VIKTIGT - applösenordet (MAIL_AUTH_PW="ditt-app-password") sätts utan mellanslag.
