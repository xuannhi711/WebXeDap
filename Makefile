# Functions trong Makefile: https://www.gnu.org/software/make/manual/html_node/Functions.html


.DEFAULT_GOAL := server

# --------------------------------------------------------------------
# ------------------ IMPLICIT GUARD ----------------------------------
# --------------------------------------------------------------------

# Pattern được dùng nhiều hồi xưa. Cách dùng:
# - đặt làm prerequisite cho target nào đó cần biến môi trường.
guard-%:
	$(if $($*),,$(error Missing required variable: $*))


# --------------------------------------------------------------------
# ------------------ ENV VARIABLES -----------------------------------
# --------------------------------------------------------------------
include .env

# Fallbacks when variables are omitted from .env or shell environment.
ASPNETCORE_ENVIRONMENT ?= Development
MSSQL_HOST ?= localhost
MSSQL_PORT ?= 1433
MSSQL_DB ?= BicycleShop
MSSQL_USER ?= sa
MSSQL_SA_PASSWORD ?= 2Secure@Password2
MSSQL_TRUST_SERVER_CERTIFICATE ?= True
MSSQL_ENCRYPT ?= False

ifndef MSSQL_SA_PASSWORD
$(error MSSQL_SA_PASSWORD is not set. Please set it in the .env file)
endif
CONNECTION_STRING := Server=$(MSSQL_HOST),$(MSSQL_PORT);\
					Database=$(MSSQL_DB);\
					User Id=$(MSSQL_USER);\
					Password=$(MSSQL_SA_PASSWORD);\
					TrustServerCertificate=$(MSSQL_TRUST_SERVER_CERTIFICATE);\
					Encrypt=$(MSSQL_ENCRYPT)






# --------------------------------------------------------------------
# ------------------ RULES -------------------------------------------
# --------------------------------------------------------------------

.PHONY=db
db:
	@podman compose up -d mssql

.PHONY=stop-db
stop-db:
	@podman compose stop mssql

.PHONY=server
server:
	@echo "Starting server..."
	@ASPNETCORE_ENVIRONMENT=$(ASPNETCORE_ENVIRONMENT) \
	ConnectionStrings__DefaultConnection="$(CONNECTION_STRING)" \
	dotnet watch --project WebXeDap.WebAPI

.PHONY=migrate
migrate:
	@echo "Applying database migrations..."
	@ConnectionStrings__DefaultConnection="$(CONNECTION_STRING)" \
	dotnet ef database update --project WebXeDap.Infrastructure

.PHONY=seed
seed:
	@echo "Seeding admin user..."
	@CONNECTION_STRING="$(CONNECTION_STRING)" \
	dotnet run --project WebXeDap.Seeder

.PHONY=test
test:
	@dotnet test

.PHONY=clean
clean:
	@dotnet clean