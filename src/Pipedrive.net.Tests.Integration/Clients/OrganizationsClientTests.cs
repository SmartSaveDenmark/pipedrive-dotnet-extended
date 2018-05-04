﻿using System.Threading.Tasks;
using Xunit;

namespace Pipedrive.Tests.Integration.Clients
{
    public class OrganizationsClientTests
    {
        public class TheGetAllMethod
        {
            [IntegrationTest]
            public async Task ReturnsCorrectCountWithoutStart()
            {
                var pipedrive = Helper.GetAuthenticatedClient();

                var options = new OrganizationFilters
                {
                    PageSize = 3,
                    PageCount = 1
                };

                var organizations = await pipedrive.Organization.GetAll(options);
                Assert.Equal(3, organizations.Count);
            }

            [IntegrationTest]
            public async Task ReturnsCorrectCountWithStart()
            {
                var pipedrive = Helper.GetAuthenticatedClient();

                var options = new OrganizationFilters
                {
                    PageSize = 2,
                    PageCount = 1,
                    StartPage = 1
                };

                var organizations = await pipedrive.Organization.GetAll(options);
                Assert.Equal(2, organizations.Count);
            }

            [IntegrationTest]
            public async Task ReturnsDistinctInfosBasedOnStartPage()
            {
                var pipedrive = Helper.GetAuthenticatedClient();

                var startOptions = new OrganizationFilters
                {
                    PageSize = 1,
                    PageCount = 1
                };

                var firstPage = await pipedrive.Organization.GetAll(startOptions);

                var skipStartOptions = new OrganizationFilters
                {
                    PageSize = 1,
                    PageCount = 1,
                    StartPage = 1
                };

                var secondPage = await pipedrive.Organization.GetAll(skipStartOptions);

                Assert.NotEqual(firstPage[0].Id, secondPage[0].Id);
            }
        }

        public class TheCreateMethod
        {
            [IntegrationTest]
            public async Task CanCreate()
            {
                var pipedrive = Helper.GetAuthenticatedClient();
                var fixture = pipedrive.Organization;

                var newOrganization = new NewOrganization("name");

                var organization = await fixture.Create(newOrganization);
                Assert.NotNull(organization);

                var retrieved = await fixture.Get(organization.Id);
                Assert.NotNull(retrieved);
            }
        }

        public class TheEditMethod
        {
            [IntegrationTest]
            public async Task CanEdit()
            {
                var pipedrive = Helper.GetAuthenticatedClient();
                var fixture = pipedrive.Organization;

                var newOrganization = new NewOrganization("new-name");
                var organization = await fixture.Create(newOrganization);

                var editOrganization = organization.ToUpdate();
                editOrganization.Name = "updated-name";
                editOrganization.VisibleTo = Visibility.shared;

                var updatedOrganization = await fixture.Edit(organization.Id, editOrganization);

                Assert.Equal("updated-name", updatedOrganization.Name);
                Assert.Equal(Visibility.shared, updatedOrganization.VisibleTo);
            }
        }

        public class TheDeleteMethod
        {
            [IntegrationTest]
            public async Task CanDelete()
            {
                var pipedrive = Helper.GetAuthenticatedClient();
                var fixture = pipedrive.Organization;

                var newOrganization = new NewOrganization("new-name");
                var organization = await fixture.Create(newOrganization);

                var createdOrganization = await fixture.Get(organization.Id);

                Assert.NotNull(createdOrganization);

                await fixture.Delete(createdOrganization.Id);

                var deletedOrganization = await fixture.Get(createdOrganization.Id);

                Assert.False(deletedOrganization.ActiveFlag);
            }
        }
    }
}