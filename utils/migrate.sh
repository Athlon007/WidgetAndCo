# This script runs dotnet ef database update to apply the latest migrations to the database.

echo "Current path: $PWD"

# Check if initial migration file exists.
# can be any date, so check by end of the file name
if [ -f "WidgetAndCo.Data/Migrations/00000000000000_CreateIdentitySchema.cs" ]; then
    echo "Initial migration file already exists."
else
    echo "Creating initial migration file..."
    dotnet ef migrations add CreateIdentitySchema -o Migrations --project WidgetAndCo.Data --startup-project WidgetAndCo
fi

echo "Applying latest migration..."

dotnet ef database update --project WidgetAndCo.Data --startup-project WidgetAndCo

echo "Migration complete."