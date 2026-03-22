## [US-119] Tests unitaires des services OAuth

**En tant que** developpeur,
**je veux** une couverture de tests unitaires complete sur les services OAuth,
**afin de** garantir la fiabilite du flux d'authentification.

### Criteres d'acceptation

- [ ] CA1 : Tests du service OAuth (ou equivalent) :
  - `FindOrCreateUser_WhenNewProvider_ShouldCreateUserAndExternalLogin`
  - `FindOrCreateUser_WhenExistingExternalLogin_ShouldReturnExistingUser`
  - `FindOrCreateUser_WhenEmailMatchesExistingUser_ShouldLinkToExistingUser`
  - `FindOrCreateUser_WhenConsentEmail_ShouldStoreEmail`
  - `FindOrCreateUser_WhenNoEmailConsent_ShouldNotStoreEmail`
- [ ] CA2 : Tests du service de code a usage unique :
  - `GenerateCode_ShouldReturnUniqueCode`
  - `ValidateCode_WhenValid_ShouldReturnStoredData`
  - `ValidateCode_WhenExpired_ShouldReturnNull`
  - `ValidateCode_WhenAlreadyUsed_ShouldReturnNull`
  - `ValidateCode_WhenUnknown_ShouldReturnNull`
- [ ] CA3 : Tests de la generation JWT mise a jour :
  - `GenerateAccessToken_WhenEmailNull_ShouldOmitEmailClaim`
  - `GenerateAccessToken_WhenEmailPresent_ShouldIncludeEmailClaim`
- [ ] CA4 : Tous les tests suivent la convention de nommage `<Method>_When<Condition>_Should<Outcome>`.
- [ ] CA5 : Les tests utilisent xUnit + Moq + Shouldly.
- [ ] CA6 : Tous les tests passent (`dotnet test garden-assistant-tests`).

### Notes & contraintes
- Les tests mockent les dependances (DbContext, IMemoryCache, etc.) — pas de base de donnees reelle.
- Couvre le service OAuth, le service de code a usage unique, et la generation JWT modifiee.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
