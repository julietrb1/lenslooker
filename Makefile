build-investigator:
	docker buildx build --platform linux/amd64 -t registry.digitalocean.com/floating-barrels/lenslooker-investigator:latest -f LensLooker/Dockerfile .

push-investigator:
	docker push registry.digitalocean.com/floating-barrels/lenslooker-investigator:latest

	
build-site:
	docker buildx build --platform linux/amd64 -t registry.digitalocean.com/floating-barrels/lenslooker-site:latest -f LensLooker.Site/Dockerfile .
	
push-site:
	docker push registry.digitalocean.com/floating-barrels/lenslooker-site:latest
	
build-push-all: build-investigator push-investigator build-site push-site