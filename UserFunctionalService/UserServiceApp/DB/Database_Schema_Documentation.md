# Database Schema Documentation

## 1. User Table
- **UserId (int, Primary Key)**: The unique identifier for each user.
- **FirstName (string, Required)**: The first name of the user.
- **LastName (string, Required)**: The last name of the user.
- **PhoneNumber (int, Unique)**: The phone number of the user, which must be unique across all users.
- **MedicineCabinetUsersList (ICollection\<MedicineCabinetUsers\>)**: Navigation property representing the many-to-many relationship between users and medicine cabinets through the `MedicineCabinetUsers` table.


## 2. MedicineCabinet Table
- **Id (int, Primary Key)**: The unique identifier for each medicine cabinet.
- **MedicineCabinetName (string, Required)**: The name of the medicine cabinet.
- **CreatorId (int, Foreign Key)**: The `UserId` of the user who created the medicine cabinet.
- **Creator (User, Optional)**: Navigation property for the user who created the medicine cabinet.
- **MedicineCabinetUsers (ICollection\<MedicineCabinetUsers\>)**: Navigation property representing the many-to-many relationship between medicine cabinets and users through the `MedicineCabinetUsers` table.
- **Medications (ICollection\<UserMedications\>)**: Navigation property representing the one-to-many relationship between medicine cabinets and the medications they contain.
- **Relationships**:
  - `CreatorId` is a foreign key referencing the `UserId` in the `User` table. The relationship is optional, and if the creator is specified, it enforces a restriction on delete (`DeleteBehavior.Restrict`), meaning the medicine cabinet cannot be deleted if it is still referenced by any users.


## 3. MedicationRepo Table
- **Id (int, Primary Key)**: The unique identifier for each medication in the repository.
- **Barcode (string, Required)**: The unique barcode of the medication.
- **DrugEnglishName (string, Required)**: The English name of the medication.
- **DrugHebrewName (string, Required)**: The Hebrew name of the medication.
- **EnglishDescription (string, Optional)**: A description of the medication in English.
- **HebrewDescription (string, Optional)**: A description of the medication in Hebrew.
- **ImagePath (string, Optional)**: The path to an image of the medication.
- **Medications (ICollection\<UserMedications\>)**: Navigation property representing the one-to-many relationship between the medication repository and user medications.
- **Note**: The `MedicationRepo` table stores the metadata of medications. This includes information such as the barcode, drug names in different languages, descriptions, and image paths, but not the specific details related to a user's personal medication (e.g., dosage, validity).


## 4. UserMedications Table
- **Id (int, Primary Key)**: The unique identifier for each user's medication.
- **Barcode (string, Required)**: The barcode of the medication (reference to the `MedicationRepo`).
- **PillSize (int?, Optional)**: The size of the pill (optional).
- **Validity (DateTime, Required)**: The expiration date of the medication.
- **CreatorId (int, Foreign Key)**: The `UserId` of the user who added the medication.
- **Creator (User, Required)**: Navigation property for the user who added the medication.
- **MedicineCabinetId (int, Foreign Key)**: The ID of the medicine cabinet to which the medication belongs.
- **MedicineCabinet (MedicineCabinet, Required)**: Navigation property for the medicine cabinet.
- **MedicationRepoId (int, Foreign Key)**: The ID of the medication in the repository.
- **MedicationRepo (MedicationRepo, Required)**: Navigation property for the medication repository.
- **IsPrivate (bool, Required)**: Indicates whether the medication is private or public.
- **Relationships**:
  - `CreatorId` is a foreign key referencing the `UserId` in the `User` table.
  - `MedicineCabinetId` is a foreign key referencing the `Id` in the `MedicineCabinet` table.
  - `MedicationRepoId` is a foreign key referencing the `Id` in the `MedicationRepo` table.


## 5. MedicineCabinetUsers Table
- **Id (int, Primary Key)**: The unique identifier for each entry in the many-to-many relationship between users and medicine cabinets.
- **UserId (int, Foreign Key)**: The `UserId` of the user.
- **User (User, Required)**: Navigation property for the user.
- **MedicineCabinetId (int, Foreign Key)**: The ID of the medicine cabinet.
- **MedicineCabinet (MedicineCabinet, Required)**: Navigation property for the medicine cabinet.
- **Relationships**:
  - `UserId` is a foreign key referencing the `UserId` in the `User` table.
  - `MedicineCabinetId` is a foreign key referencing the `Id` in the `MedicineCabinet` table.
  - This table represents a many-to-many relationship between `User` and `MedicineCabinet`, where a user can belong to multiple medicine cabinets, and a medicine cabinet can have multiple users.


## 6. CabinetRequest Table
- **Id (int, Primary Key)**: The unique identifier for each request.
- **TargetPhoneNumber (string?, Optional)**: The phone number of the target user.
- **SenderPhoneNumber (string, Required)**: The phone number of the user who sent the request.
- **IsHandle (bool, Required)**: Indicates whether the request has been handled.
- **IsApprove (bool, Required)**: Indicates whether the request has been approved.
- **IsSenderSeen (bool, Required)**: Indicates whether the sender has seen the response to the request.
- **DateStart (DateTime, Required)**: The date when the request was created.
- **DateEnd (DateTime, Required)**: The date when the request was completed or closed.

