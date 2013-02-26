
EXTRA_DIST = Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/Makefile.include
include $(top_srcdir)/config.make

#include $(srcdir)/custom-hooks.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug -define:DEBUG
ASSEMBLY = bin/Debug/salmon-viewer.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

TAO_OPENGL_DLL_SOURCE=lib/Tao.OpenGl.dll
TAO_FREEGLUT_DLL_SOURCE=lib/Tao.FreeGlut.dll
TAO_OPENGL_DLL_CONFIG_SOURCE=lib/Tao.OpenGl.dll.config
TAO_FREEGLUT_DLL_CONFIG_SOURCE=lib/Tao.FreeGlut.dll.config

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/salmon-viewer.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

TAO_OPENGL_DLL_SOURCE=lib/Tao.OpenGl.dll
TAO_FREEGLUT_DLL_SOURCE=lib/Tao.FreeGlut.dll
TAO_OPENGL_DLL_CONFIG_SOURCE=lib/Tao.OpenGl.dll.config
TAO_FREEGLUT_DLL_CONFIG_SOURCE=lib/Tao.FreeGlut.dll.config

endif


PROGRAMFILES = \
	$(TAO_OPENGL_DLL) \
	$(TAO_FREEGLUT_DLL) \
	$(TAO_OPENGL_DLL_CONFIG) \
	$(TAO_FREEGLUT_DLL_CONFIG)  

BINARIES = \
	$(SALMON_VIEWER)  



TAO_OPENGL_DLL = $(BUILD_DIR)/Tao.OpenGl.dll
SALMON_VIEWER = $(BUILD_DIR)/salmon-viewer
TAO_FREEGLUT_DLL = $(BUILD_DIR)/Tao.FreeGlut.dll
TAO_OPENGL_DLL_CONFIG = $(BUILD_DIR)/Tao.OpenGl.dll.config
TAO_FREEGLUT_DLL_CONFIG = $(BUILD_DIR)/Tao.FreeGlut.dll.config


FILES = \
	Entity.cs \
	Program.cs \
	Material.cs \
	Matrix.cs \
	Model.cs \
	ThreeDSFile.cs \
	Triangle.cs \
	Vector.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	AUTHORS \
	COPYING \
	lib/Tao.FreeGlut.dll \
	lib/Tao.FreeGlut.dll.config \
	lib/Tao.FreeGlut.xml \
	lib/Tao.OpenGl.dll \
	lib/Tao.OpenGl.dll.config \
	lib/Tao.OpenGl.xml \
	salmon-viewer.in 

REFERENCES =  \
	System.Drawing

DLL_REFERENCES =  \
	lib/Tao.FreeGlut.dll \
	lib/Tao.OpenGl.dll

CLEANFILES += $(PROGRAMFILES) $(BINARIES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make

$(TAO_OPENGL_DLL): $(TAO_OPENGL_DLL_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

$(SALMON_VIEWER): salmon-viewer
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'
	chmod u+x '$@'

$(TAO_FREEGLUT_DLL): $(TAO_FREEGLUT_DLL_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

$(TAO_OPENGL_DLL_CONFIG): $(TAO_OPENGL_DLL_CONFIG_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

$(TAO_FREEGLUT_DLL_CONFIG): $(TAO_FREEGLUT_DLL_CONFIG_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



salmon-viewer: salmon-viewer.in $(top_srcdir)/config.make
	sed -e "s,@prefix@,$(prefix)," -e "s,@PACKAGE@,$(PACKAGE)," < salmon-viewer.in > salmon-viewer


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'



$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(dir $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)


install-local: $(ASSEMBLY) $(ASSEMBLY_MDB) $(TAO_OPENGL_DLL) $(SALMON_VIEWER) $(TAO_FREEGLUT_DLL) $(TAO_OPENGL_DLL_CONFIG) $(TAO_FREEGLUT_DLL_CONFIG)
	make pre-install-local-hook prefix=$(prefix)
	mkdir -p $(prefix)/lib/$(PACKAGE)
	cp $(ASSEMBLY) $(ASSEMBLY_MDB) $(prefix)/lib/$(PACKAGE)
	test -z '$(TAO_OPENGL_DLL)' || cp $(TAO_OPENGL_DLL) $(prefix)/lib/$(PACKAGE)
	mkdir -p $(prefix)/bin
	test -z '$(SALMON_VIEWER)' || cp $(SALMON_VIEWER) $(prefix)/bin
	test -z '$(TAO_FREEGLUT_DLL)' || cp $(TAO_FREEGLUT_DLL) $(prefix)/lib/$(PACKAGE)
	test -z '$(TAO_OPENGL_DLL_CONFIG)' || cp $(TAO_OPENGL_DLL_CONFIG) $(prefix)/lib/$(PACKAGE)
	test -z '$(TAO_FREEGLUT_DLL_CONFIG)' || cp $(TAO_FREEGLUT_DLL_CONFIG) $(prefix)/lib/$(PACKAGE)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB) $(TAO_OPENGL_DLL) $(SALMON_VIEWER) $(TAO_FREEGLUT_DLL) $(TAO_OPENGL_DLL_CONFIG) $(TAO_FREEGLUT_DLL_CONFIG)
	make pre-uninstall-local-hook prefix=$(prefix)
	rm -f $(prefix)/lib/$(PACKAGE)/$(notdir $(ASSEMBLY))
	test -z '$(ASSEMBLY_MDB)' || rm -f $(prefix)/lib/$(PACKAGE)/$(notdir $(ASSEMBLY_MDB))
	test -z '$(TAO_OPENGL_DLL)' || rm -f $(prefix)/lib/$(PACKAGE)/$(notdir $(TAO_OPENGL_DLL))
	test -z '$(SALMON_VIEWER)' || rm -f $(prefix)/bin/$(notdir $(SALMON_VIEWER))
	test -z '$(TAO_FREEGLUT_DLL)' || rm -f $(prefix)/lib/$(PACKAGE)/$(notdir $(TAO_FREEGLUT_DLL))
	test -z '$(TAO_OPENGL_DLL_CONFIG)' || rm -f $(prefix)/lib/$(PACKAGE)/$(notdir $(TAO_OPENGL_DLL_CONFIG))
	test -z '$(TAO_FREEGLUT_DLL_CONFIG)' || rm -f $(prefix)/lib/$(PACKAGE)/$(notdir $(TAO_FREEGLUT_DLL_CONFIG))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
