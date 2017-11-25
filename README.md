# BekkSjakkApi

Tenarlaust API som nyttar Azure Functions.

## Korleis kalle funksjonane i API-et

API-et køyrer på URL-en [https://bekksjakkapi.azurewebsites.net](https://bekksjakkapi.azurewebsites.net).

Informasjonside om korleis API-et fungerer [finst her](https://bekksjakkapi.azurewebsites.net/api/Om).

## Tankar om Azure Functions Hosting Plan

Vertsplanen (hosting plan) bestemmer korleis Azure Functions blir køyrt. Ein kan velge mellom:

- Dedicated VM: Funksjonane køyrer på ein reservert VM. VM-en køyrer konstant, og vi betalar vanleg VM-pris.
- Consumption: Funksjonane blir køyrt på ein VM som berre køyrer når vi kallar funksjonane. Den sovnar ti minutt etter at ein funksjon har køyrt.

Vi prøvar å bruke Consumption-planen først. Ulempa er at responstida er fire til ti sekund på den første førespurnaden etter at VM-en har sovna.
Vi har to moglege work-arounds:

- Lage ein eigen funksjon som blir periodisk kalla av ein *time trigger*. Dette gjer at VM-en blir holdt våken.
- Kalle ein funksjon i starten av eit sjakkspel. Dette våknar opp VM-en, slik at påfølgande førespurnader vil gå raskt.

